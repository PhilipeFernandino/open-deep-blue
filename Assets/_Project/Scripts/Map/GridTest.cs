using Coimbra;
using Core.Map;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using Tile = Core.Map.Tile;

namespace Systems.Grid_System
{
    public class GridTest : MonoBehaviour
    {
        [Tooltip("Size of the grid from -thisValue to thisValue")]
        [SerializeField] private int _spritesPoolSize;
        [SerializeField] private Vector2 _gridDrawOffset = Vector2.zero;
        [SerializeField] private Tilemap _tilemap;
        [SerializeField] private MapSystem _mapSystem;

        [SerializeField] private SpriteRenderer _gridSpriteRendererPrefab;

        private List<SpriteRenderer> _gridSprites;

        private TileInstance[,] _grid;
        private TilesSettings _tilesSettings;

        private int _offset;
        private int _gridSize;
        private int _lastGridDrawSize = 0;
        private bool _isInitialized = false;

        private void Update()
        {
            var mousePos = (Vector3)Mouse.current.position.ReadValue();
            mousePos.z = 10f;
            var pos = Camera.main.ScreenToWorldPoint((Vector3)Mouse.current.position.ReadValue());
            pos.z = 0f;

            DrawInGrid(pos, new Vector2Int(1, 1));
        }

        public void DrawInGrid(Vector2 position, in Vector2Int size)
        {
            if (!_isInitialized)
                return;

            var rounded = new Vector2Int(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y));

            for (int i = 0; i < size.x; i++)
            {
                for (int j = 0; j < size.y; j++)
                {
                    int x = rounded.x + i;
                    int y = rounded.y + j;

                    int index = j + i * size.y;

                    _gridSprites[index].transform.position = new Vector2(x, y) + _gridDrawOffset;

                    if (TryGetTileAt(x, y, out TileInstance tile))
                    {
                        _gridSprites[index].color = Color.blue;
                        Debug.Log($"{tile} at {x}, {y}, {_mapSystem.GetTile(x, y)}, {_tilesSettings.GetDefinition(_mapSystem.GetTile(x, y))}");
                    }
                    else
                    {
                        _gridSprites[index].color = Color.red;
                    }

                    _gridSprites[index].enabled = true;
                }
            }

            int gridDrawSize = size.x * size.y;
            for (int i = gridDrawSize; i < _lastGridDrawSize; i++)
            {
                _gridSprites[i].enabled = false;
            }
        }

        public bool TryGetTileAt(int x, int y, out TileInstance tile)
        {
            int xGrid = x;
            int yGrid = y;

            if ((xGrid < 0 || xGrid >= _gridSize) || (yGrid < 0 || yGrid >= _gridSize))
            {
                tile = TileInstance.None;
                return false;
            }

            tile = _grid[xGrid, yGrid];
            return true;
        }

        private void Awake()
        {
            _tilesSettings = ScriptableSettings.GetOrFind<TilesSettings>();
        }

        private void Start()
        {
            _mapSystem.MapLoaded += MapLoadedEventHandler;
        }

        private void MapLoadedEventHandler(Map map)
        {
            InitializeGrid(map);
            InitializeDrawGridSprites();
            _isInitialized = true;
        }


        private void InitializeGrid(Map map)
        {
            _gridSize = map.Metadata.Dimensions;
            _grid = new TileInstance[_gridSize, _gridSize];

            for (int i = 0; i < map.Metadata.Dimensions; i++)
            {
                for (int j = 0; j < map.Metadata.Dimensions; j++)
                {
                    _grid[i, j] = (TileInstance)_tilesSettings.GetDefinition(map.Metadata.Tiles[i, j]);
                }
            }

            _offset = 0;
            _gridDrawOffset = Vector2.zero;
        }

        private void InitializeDrawGridSprites()
        {
            _gridSprites = new(_spritesPoolSize);

            for (int i = 0; i < _spritesPoolSize; i++)
            {
                _gridSprites.Add(Instantiate(_gridSpriteRendererPrefab, transform));
                _gridSprites[i].enabled = false;
            }
        }
    }
}