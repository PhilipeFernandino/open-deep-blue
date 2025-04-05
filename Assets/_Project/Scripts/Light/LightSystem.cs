using Coimbra;
using Coimbra.Services;
using Core.Level;
using Core.Util;
using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Core.Light
{
    public class LightSystem : Actor, ILightService
    {
        [SerializeField] private Material _lightOverlayMaterial;
        [SerializeField] private MeshRenderer _meshRenderer;
        [SerializeField] private int _dimensions = 32;
        [SerializeField] private FilterMode _filterMode;

        public int[,] _lightMap;

        private IGridService _gridService;

        private Material _lightMaterial;
        private Texture2D _lightTexture;

        List<LightSource> _activeSources = new();
        private Queue<Vector2Int> _propagateLightQueue = new();

        private Vector2Int[] _neighboors = {
                Vector2Int.up, Vector2Int.down,
                Vector2Int.left, Vector2Int.right
            };

        public void AddLightSource(Vector2Int position, int intensity)
        {
            _activeSources.Add(new LightSource(position, intensity));
            PropagateLight(position, intensity);
            UpdateLightTexture();
        }

        private void PropagateLight(Vector2Int sourcePos, int intensity)
        {
            _propagateLightQueue.Clear();

            //if (_gridService.IsTileLoaded(sourcePos))
            //{

            //}

            _lightMap[sourcePos.x, sourcePos.y] = intensity;
            _propagateLightQueue.Enqueue(sourcePos);



            while (_propagateLightQueue.Count > 0)
            {
                Vector2Int current = _propagateLightQueue.Dequeue();
                int currentLevel = _lightMap[current.x, current.y];

                foreach (var dir in _neighboors)
                {
                    Vector2Int next = current + dir;
                    if (Range.IsWithinBounds(next, Vector2Int.zero, new Vector2Int()))
                        continue;

                    int newLevel = currentLevel - 1;
                    if (newLevel > _lightMap[next.x, next.y])
                    {
                        if (!_gridService.HasTileAt(current))
                        {
                            _lightMap[next.x, next.y] = newLevel;
                            if (newLevel > 1)
                            {
                                _propagateLightQueue.Enqueue(next);
                            }
                        }
                        else
                        {
                            _lightMap[next.x, next.y] += newLevel / 2;
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

        protected override void OnInitialize()
        {
            InitializeLightMap();
            InitializeLightTexture();

            _meshRenderer.sortingLayerName = "Light Overlay";

            //InitializeQuad();
        }

        protected override void OnSpawn()
        {
            _gridService = ServiceLocatorUtilities.GetServiceAssert<IGridService>();
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

        private void SetProportionalQuad()
        {
            float height = Camera.main.orthographicSize * 2;
            float width = height * Camera.main.aspect;
            _meshRenderer.transform.localScale = new Vector3(width, height, 1);
        }

        #region debug
        public int x, y, intensity;
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
