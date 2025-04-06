using Coimbra;
using Coimbra.Services;
using Core.EventBus;
using Core.Level;
using Core.Util;
using NaughtyAttributes;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Core.Light
{
    public class LightSystem : Actor, ILightService
    {
        [SerializeField] private Material _lightOverlayMaterial;
        [SerializeField] private PositionEventBus _positionEventBus;
        [SerializeField] private MeshRenderer _meshRendererPrefab;

        [SerializeField] private FilterMode _filterMode;
        [SerializeField] private int _chunkSize = 16;
        [SerializeField] private int _loadNearChunks = 1;
        [SerializeField] private int _baseIntensity;
        [SerializeField] private float _lightFallof;
        [SerializeField] private Vector2 _offset;

        private Vector2Int _origin;
        private int _dimensions;

        private int[,] _lightMap;

        private IGridService _gridService;

        private MeshRenderer _meshRenderer;

        private MeshRenderer MeshRenderer
        {
            get
            {
                if (_meshRenderer == null)
                {
                    InitializeMeshRenderer();
                }

                return _meshRenderer;
            }
        }

        private Texture2D _lightTexture;

        private ChunkController _chunkController;

        private List<LightSource> _activeSources = new();
        private Queue<Vector2Int> _propagateLightQueue = new();

        private HashSet<Vector2Int> _activeChunks;

        private Vector2Int[] _neighboors = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

        public void AddLightSource(Vector2Int position, int intensity)
        {
            _activeSources.Add(new LightSource(position, intensity));
            UpdateLightTexture();
        }

        private void PropagateLight(LightSource source)
        {
            Vector2Int sourcePos = source.Position;
            Vector2Int overlayNormPos = sourcePos - _origin;

            _propagateLightQueue.Clear();

            _lightMap[overlayNormPos.x, overlayNormPos.y] = source.Intensity;
            _propagateLightQueue.Enqueue(overlayNormPos);

            while (_propagateLightQueue.Count > 0)
            {
                Vector2Int overlayCurrentPos = _propagateLightQueue.Dequeue();

                int currentLevel = _lightMap[overlayCurrentPos.x, overlayCurrentPos.y];

                foreach (var dir in _neighboors)
                {
                    Vector2Int nextWorldPos = overlayCurrentPos + _origin + dir;
                    Vector2Int nextOverlayPos = overlayCurrentPos + dir;

                    if (!Util.Range.IsWithinBounds(nextOverlayPos, 0, _dimensions))
                    {
                        continue;
                    }

                    int newLevel = currentLevel - 1;
                    if (newLevel > _lightMap[nextOverlayPos.x, nextOverlayPos.y])
                    {
                        if (!_gridService.HasTileAt(nextWorldPos))
                        {
                            _lightMap[nextOverlayPos.x, nextOverlayPos.y] = newLevel;
                            if (newLevel > 1)
                            {
                                _propagateLightQueue.Enqueue(nextOverlayPos);
                            }
                        }
                        else
                        {
                            _lightMap[nextOverlayPos.x, nextOverlayPos.y] += (int)(newLevel * _lightFallof);
                        }
                    }
                }
            }
        }

        private void UpdateLightTexture()
        {
            SetLightmapGlobal(0);

            foreach (var source in _activeSources)
            {
                var toChunk = _chunkController.ToChunk(source.Position);
                if (_activeChunks.Contains(toChunk))
                {
                    PropagateLight(source);
                }
            }

            // Remake texture
            Color[] pixels = new Color[_dimensions * _dimensions];
            for (int x = 0; x < _dimensions; x++)
            {
                for (int y = 0; y < _dimensions; y++)
                {
                    float brightness = _lightMap[x, y] / 15f; // Normalize to 0-1
                    pixels[y * _dimensions + x] = new Color(brightness, brightness, brightness, 1);
                }
            }

            _lightTexture.SetPixels(pixels);
            _lightTexture.Apply();
            _lightOverlayMaterial.SetTexture("_LightTex", _lightTexture);
        }

        protected override void OnSpawn()
        {
            Debug.Log($"Spawned");

            _gridService = ServiceLocatorUtilities.GetServiceAssert<IGridService>();
            _dimensions = _gridService.LoadedDimensions;

            _chunkController = new(_chunkSize, _loadNearChunks);

            _chunkController.TileChunkSetted += TileChunkSetted_EventHandler;
            _chunkController.TileChunkUnsetted += TileChunkUnsetted_EventHandler;
            _chunkController.TileChunksUpdated += TileChunksUpdated_EventHandler;

            _chunkController.OriginSetted += OriginSetted_EventHandler;

            _positionEventBus.PositionChanged += _chunkController.UpdatePosition;

            InitializeLightMap();
            InitializeLightTexture();
            InitializeMeshRenderer();
        }

        private void TileChunksUpdated_EventHandler(HashSet<Vector2Int> activeChunks)
        {
            _activeChunks = activeChunks;
            UpdateLightTexture();
        }

        private void InitializeLightMap()
        {
            _lightMap = new int[_dimensions, _dimensions];
            SetLightmapGlobal(0);
        }

        private void SetLightmapGlobal(int value)
        {
            for (int x = 0; x < _dimensions; x++)
            {
                for (int y = 0; y < _dimensions; y++)
                {
                    _lightMap[x, y] = _baseIntensity;
                }
            }
        }

        private void InitializeLightTexture()
        {
            _lightTexture = new Texture2D(_dimensions, _dimensions, TextureFormat.R8, false)
            {
                wrapMode = TextureWrapMode.Clamp,
                filterMode = _filterMode
            };
            _lightOverlayMaterial.SetTexture("_LightTex", _lightTexture);
        }

        private void InitializeMeshRenderer()
        {
            _meshRenderer = Instantiate(_meshRendererPrefab, Vector3.zero, Quaternion.identity);
            _meshRenderer.material = _lightOverlayMaterial;
            _meshRenderer.sortingLayerName = "Light Overlay";
            _meshRenderer.transform.localScale = new Vector3Int(_chunkController.LoadedDimensions, _chunkController.LoadedDimensions, 1);
        }

        public void OriginSetted_EventHandler(Vector2Int origin)
        {
            MeshRenderer.transform.position = new Vector3Int(origin.x + _chunkController.LoadedDimensions / 2, origin.y + _chunkController.LoadedDimensions / 2, 0);
            _lightOverlayMaterial.SetVector("_Origin", new Vector4(origin.x + _offset.x, y: origin.y + _offset.y, 0, 0));
            _origin = origin;
        }

        public void TileChunkSetted_EventHandler((BoundsInt area, Vector2Int anchor) e)
        {
            Debug.Log(e.anchor);
        }

        public void TileChunkUnsetted_EventHandler((BoundsInt area, Vector2Int anchor) e)
        {

        }

        #region debug
        [Header("Debug")]
        [SerializeField] private int x;
        [SerializeField] private int y, intensity;
        [Button]
        public void AddLightAt()
        {
            AddLightSource(new Vector2Int(x, y), intensity);
        }

        [Button]
        public void AddLightAtPos()
        {
            AddLightSource(Vector2Int.RoundToInt(_positionEventBus.Position), intensity);
        }

        [Button]
        public void TriggerPositionBus()
        {
            _positionEventBus.Position = new Vector2Int(x, y);
            _positionEventBus.Trigger();
        }
        #endregion
    }

    [DynamicService]
    public interface ILightService : IService
    {

    }
}
