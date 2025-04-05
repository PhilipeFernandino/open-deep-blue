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
    public class LightSystem : Actor, ILightService, IChunkController
    {
        [SerializeField] private Material _lightOverlayMaterial;
        [SerializeField] private MeshRenderer _meshRenderer;
        [SerializeField] private PositionEventBus _positionEventBus;

        [SerializeField] private FilterMode _filterMode;
        [SerializeField] private int _chunkSize = 16;
        [SerializeField] private int _loadNearChunks = 1;

        private Vector2Int _origin;
        private int _dimensions;

        private int[,] _lightMap;

        private IGridService _gridService;

        private Material _lightMaterial;
        private Texture2D _lightTexture;

        private ChunkController _chunkController;

        private List<LightSource> _activeSources = new();
        private Queue<Vector2Int> _propagateLightQueue = new();

        private Vector2Int[] _neighboors = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

        public void AddLightSource(Vector2Int position, int intensity)
        {
            _activeSources.Add(new LightSource(position, intensity));
            PropagateLight(position, intensity);
            UpdateLightTexture();
        }

        private void PropagateLight(Vector2Int sourcePos, int intensity)
        {
            Vector2Int overlayNormPos = sourcePos - _origin;

            _propagateLightQueue.Clear();

            _lightMap[overlayNormPos.x, overlayNormPos.y] = intensity;
            _propagateLightQueue.Enqueue(overlayNormPos);

            while (_propagateLightQueue.Count > 0)
            {
                Vector2Int overlayCurrentPos = _propagateLightQueue.Dequeue();

                int currentLevel = _lightMap[overlayCurrentPos.x, overlayCurrentPos.y];

                foreach (var dir in _neighboors)
                {
                    Vector2Int nextWorldPos = overlayCurrentPos + _origin + dir;
                    Vector2Int nextOverlayPos = overlayCurrentPos + dir;

                    //if (Range.IsWithinBounds(nextWorldPos, Vector2Int.zero, ))
                    //    continue;

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
                            _lightMap[nextOverlayPos.x, nextOverlayPos.y] += newLevel / 2;
                        }
                    }
                }
            }
        }

        private void UpdateLightTexture()
        {
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
            _gridService = ServiceLocatorUtilities.GetServiceAssert<IGridService>();
            _dimensions = _gridService.LoadedDimensions;

            _chunkController = new(_chunkSize, _loadNearChunks, this);

            _positionEventBus.PositionChanged += _chunkController.UpdatePosition;

            InitializeLightMap();
            InitializeLightTexture();
            InitializeMeshRenderer();
        }

        private void InitializeLightMap()
        {
            _lightMap = new int[_dimensions, _dimensions];

            // Optional: Initialize with ambient light (e.g., zero for caves)
            for (int x = 0; x < _dimensions; x++)
            {
                for (int y = 0; y < _dimensions; y++)
                {
                    _lightMap[x, y] = 0;
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
            _meshRenderer.sortingLayerName = "Light Overlay";
            _meshRenderer.transform.localScale = new Vector3Int(_chunkController.LoadedDimensions, _chunkController.LoadedDimensions, 1);
        }

        public void SetOrigin(Vector2Int origin)
        {
            _meshRenderer.transform.position = new Vector3Int(origin.x + _chunkController.LoadedDimensions / 2, origin.y + _chunkController.LoadedDimensions / 2, 0);
            _lightOverlayMaterial.SetVector("_Origin", new Vector4(origin.x, y: origin.y, 0, 0));
            _origin = origin;
        }

        public void SetTileChunk(BoundsInt area, Vector2Int anchor)
        {
            Debug.Log(anchor);
        }

        public void UnsetTileChunk(BoundsInt area, Vector2Int anchor)
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
        #endregion
    }

    [DynamicService]
    public interface ILightService : IService
    {

    }
}
