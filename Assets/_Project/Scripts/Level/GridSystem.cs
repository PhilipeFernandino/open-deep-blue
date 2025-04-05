using Coimbra;
using Coimbra.Services;
using Coimbra.Services.Events;
using Core.EventBus;
using Core.Map;
using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;
using static Core.Util.Range;

namespace Core.Level
{
    public class GridSystem : Actor, IGridService
    {
        [Tooltip("Size of the grid from -thisValue to thisValue")]
        [SerializeField] private int _spritesPoolSize;
        [SerializeField] private Tilemap _tilemap;
        [SerializeField] private Tilemap _floorTilemap;
        [SerializeField] private MapSystem _mapSystem;
        [SerializeField] private SpriteRenderer _gridSpriteRendererPrefab;

        [Header("Chunk Control")]
        [SerializeField] private int _chunkSize;
        [SerializeField] private int _loadNearChunks;
        [SerializeField] private PositionEventBus _positionEventBus;

        private List<SpriteRenderer> _gridSprites;

        private TileInstance[,] _grid;
        private TileChunkController _chunkController;
        private TilesSettings _tilesSettings;

        private int _gridSize;
        private int _lastGridDrawSize = 0;
        private bool _isInitialized = false;

        private MapMetadata _mapMetadata;

        public int ChunkSize => _chunkSize;
        public int LoadedDimensions => _chunkSize * (_loadNearChunks * 2 + 1);
        public int MapDimensions => _mapMetadata.Dimensions;

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

                    _gridSprites[index].transform.position = new Vector2(x, y);

                    if (TryGetTileAt(x, y, out TileInstance tile))
                    {
                        _gridSprites[index].color = Color.blue;
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
            if (!IsWithinBounds(x, y, 0, _mapMetadata.Dimensions))
            {
                tile = TileInstance.None;
                return false;
            }

            tile = _grid[x, y];
            return true;
        }

        public void DamageTileAt(int x, int y, ushort damage)
        {
            Debug.Log($"{GetType()} - DamageTileAt - {x}, {y}, {damage}");
            int currentHitPoints = Mathf.Max(0, (int)_grid[x, y].CurrentHitPoints - (int)damage);
            if (currentHitPoints <= 0)
            {
                SetTileAt(x, y, null);
            }
        }

        public bool HasTileAt(int x, int y)
        {
            return IsWithinBounds(x, y, 0, _mapMetadata.Dimensions) && _grid[x, y] != TileInstance.None;
        }

        public bool IsTileLoaded(int x, int y)
        {
            return IsWithinBounds(x, y, 0, _mapMetadata.Dimensions); // load from chunk
        }

        public void SetTileAt(int x, int y, TileBase tileBase)
        {
            _tilemap.SetTile(new Vector3Int(x, y, 0), tileBase);
        }

        protected override void OnInitialize()
        {
            ServiceLocator.Set<IGridService>(this);
        }

        protected override void OnSpawn()
        {
            _tilesSettings = ScriptableSettings.GetOrFind<TilesSettings>();
            MapMetadataGeneratedEvent.AddListener(MapLoaded_EventHandler);
        }

        private void MapLoaded_EventHandler(ref EventContext context, in MapMetadataGeneratedEvent e)
        {
            InitializeGrid(e.MapMetadata);
            InitializeDrawGridSprites();
            _isInitialized = true;
            _mapMetadata = e.MapMetadata;
            _chunkController = new(_mapMetadata, _tilemap, _floorTilemap,
                _chunkSize, _loadNearChunks, _positionEventBus, _tilesSettings);
        }

        private void InitializeGrid(MapMetadata mapMetadata)
        {
            _gridSize = mapMetadata.Dimensions;
            _grid = new TileInstance[_gridSize, _gridSize];

            for (int i = 0; i < mapMetadata.Dimensions; i++)
            {
                for (int j = 0; j < mapMetadata.Dimensions; j++)
                {
                    _grid[i, j] = (TileInstance)_tilesSettings.GetDefinition(mapMetadata.Tiles[i, j]);
                }
            }
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

        [Header("Debug")]
        [SerializeField] private int _clearDimensions;

        [Button]
        private void ClearTiles()
        {
            for (int i = 0; i < _clearDimensions; i++)
            {
                for (int j = 0; j <= _clearDimensions; j++)
                {

                    _tilemap.SetTile(new Vector3Int(i, j, 0), null);
                }
            }
        }
    }
}