using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Core.Light
{
    [System.Serializable]
    public class LightSource
    {
        public Vector3Int Position; // Tilemap position (e.g., obstacleTilemap.WorldToCell(worldPos))
        public int Intensity;       // Light strength (e.g., 15 for full brightness)
        public Color Color;         // Optional: For colored lights


        public LightSource(Vector3Int pos, int intensity, Color color = default)
        {
            Position = pos;
            Intensity = intensity;
            Color = color == default ? Color.white : color;
        }

    }
    public class LightSystem : MonoBehaviour
    {
        [SerializeField] private Tilemap _groundTilemap;
        [SerializeField] private Tilemap _wallTilemap;
        [SerializeField] private Material _lightOverlayMaterial;
        [SerializeField] private MeshRenderer _meshRenderer;
        [SerializeField] private int _dimensions = 32;
        [SerializeField] private FilterMode _filterMode;

        public int[,] _lightMap;

        private Material _lightMaterial;
        private Texture2D _lightTexture;

        List<LightSource> _activeSources = new List<LightSource>();

        #region debug
        public int x, y, intensity;
        [Button]
        public void AddLightAt()
        {
            AddLightSource(new Vector3Int(x, y, 0), intensity);
        }

        #endregion

        public void AddLightSource(Vector3Int position, int intensity)
        {
            _activeSources.Add(new LightSource(position, intensity));
            PropagateLight(position, intensity);
            UpdateLightTexture(); // Refresh visual representation
        }

        private bool IsTileSolid(Vector3Int position)
        {
            return _wallTilemap.HasTile(position);
        }

        private void PropagateLight(Vector3Int sourcePos, int intensity)
        {
            Queue<Vector3Int> queue = new Queue<Vector3Int>();
            _lightMap[sourcePos.x, sourcePos.y] = intensity;
            queue.Enqueue(sourcePos);

            Vector3Int[] directions = {
                Vector3Int.up, Vector3Int.down,
                Vector3Int.left, Vector3Int.right
            };

            while (queue.Count > 0)
            {
                Vector3Int current = queue.Dequeue();
                int currentLevel = _lightMap[current.x, current.y];

                foreach (Vector3Int dir in directions)
                { // Up, down, left, right
                    Vector3Int next = current + dir;
                    if (IsOutOfBounds(next))
                        continue;

                    int newLevel = currentLevel - 1;
                    if (newLevel > _lightMap[next.x, next.y])
                    {
                        if (!IsTileSolid(current))
                        {

                            _lightMap[next.x, next.y] = newLevel;
                            if (newLevel > 1)
                                queue.Enqueue(next);
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
            // Convert lightMap to texture colors
            Color[] pixels = new Color[_dimensions * _dimensions];
            for (int x = 0; x < _dimensions; x++)
            {
                for (int y = 0; y < _dimensions; y++)
                {
                    float brightness = _lightMap[x, y] / 15f; // Normalize to 0-1
                    pixels[y * _dimensions + x] = new Color(brightness, brightness, brightness, 1);
                }
            }

            // Apply to texture
            _lightTexture.SetPixels(pixels);
            _lightTexture.Apply();
            _lightOverlayMaterial.SetTexture("_LightTex", _lightTexture);
        }

        private void Start()
        {
            InitializeLightMap();
            InitializeLightTexture();
            InitializeQuad();
        }

        private void InitializeLightMap()
        {
            // For a dimxdim grid:
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

        private bool IsOutOfBounds(Vector3Int pos)
        {
            return pos.x < 0 || pos.x >= _dimensions || pos.y < 0 || pos.y >= _dimensions;
        }

        private bool IsInBounds(Vector3Int pos)
        {
            return !IsOutOfBounds(pos);
        }


        private void InitializeQuad()
        {
            float height = Camera.main.orthographicSize * 2;
            float width = height * Camera.main.aspect;
            //_meshRenderer.transform.localScale = new Vector3(width, height, 1);
        }
    }
}
