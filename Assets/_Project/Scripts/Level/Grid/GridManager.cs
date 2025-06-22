using Coimbra;
using Coimbra.Services;
using Coimbra.Services.Events;
using Core.Debugger;
using Core.EventBus;
using Core.Map;
using NaughtyAttributes;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;
using static Core.Util.Range;
using Tile = Core.Map.Tile;

namespace Core.Level
{
    public class GridManager : Actor, IGridService
    {
        [Tooltip("Size of the grid from -thisValue to thisValue")]
        [SerializeField] private int _spritesPoolSize;
        [SerializeField] private Tilemap _tilemap;
        [SerializeField] private Tilemap _floorTilemap;
        [SerializeField] private SpriteRenderer _gridSpriteRendererPrefab;
        [SerializeField] private bool _useChunkLoad;

        [Header("Chunk Control")]
        [SerializeField] private int _chunkSize;
        [SerializeField] private int _loadNearChunks;
        [SerializeField] private PositionEventBus _positionEventBus;

        [Header("Debugging")]
        [SerializeField] private DebugChannelSO _debugChannel;
        [SerializeField] private bool _debug = true;

        public TileInstance[,] Grid => _grid;

        private TileInstance[,] _grid;
        private TileChunkController _chunkController;
        private TilesSettings _tilesSettings;
        private GridDrawer _gridDrawer;

        private int _gridSize;

        private MapMetadata _mapMetadata;

        public int ChunkSize => _chunkSize;
        public int LoadedDimensions => _chunkSize * (_loadNearChunks * 2 + 1);
        public int Dimensions => _mapMetadata.Dimensions;


        public event Action Initialized;
        public event Action<int, int, TileInstance, TileDefinition> TileChanged;


        public void ListPositions(Tile tile, List<Vector2Int> listPositions)
        {
            listPositions.Clear();

            for (int i = 0; i < Dimensions; i++)
            {
                for (int j = 0; j < Dimensions; j++)
                {
                    Tile compareTile = Get(i, j).TileType;
                    if (compareTile == tile)
                    {
                        listPositions.Add(new(i, j));
                    }
                }
            }
        }

        public void ClearDrawAt(Vector2 position) => _gridDrawer?.ClearAt(position);
        public void DrawInGrid(Vector2 position, Color color) => _gridDrawer?.DrawInGrid(position, color);
        public void DrawInGrid(Vector2 position, in Vector2Int size, Color color) => _gridDrawer?.DrawInGrid(position, size, color);

        private void Update()
        {
            RaiseDebug();
        }

        private void RaiseDebug()
        {
            if (!_debug)
                return;

            var mousePos = Mouse.current.position.ReadValue();
            var pos = Camera.main.ScreenToWorldPoint(mousePos);
            pos.z = 0f;

            var (x, y) = IGridService.ToGridPosition(pos);
            TryGetTileAt(x, y, out var tileInstance);

            _debugChannel.RaiseEvent("grid", new GridDebugData()
            {
                TilemapName = _mapMetadata.Name,
                Dimensions = _mapMetadata.Dimensions,
                TilePosition = new Vector2Int(x, y),
                TileInstance = tileInstance,
                TileDefiniton = _tilesSettings.GetDefinition(tileInstance.TileType),
            });
        }

        public TileInstance GetTileAt(Vector3 pos)
        {
            var v2 = Vector2Int.FloorToInt(pos.XY());
            if (IsWithinBounds(v2.x, v2.y, 0, _gridSize))
                return _grid[v2.x, v2.y];
            return Map.TileInstance.None;
        }

        public TileInstance Get(int x, int y)
        {
            if (TryGetTileAt(x, y, out TileInstance tile))
            {
                return tile;
            }

            return TileInstance.None;
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

        public void DamageTileAt(int x, int y, float damage)
        {
            Debug.Log($"Damage tile: {damage} at {x}, {y}", this);
            if (TryGetTileAt(x, y, out var tile))
            {
                var tileDef = _tilesSettings.GetDefinition(tile.TileType);
                float currentHitPoints = Mathf.Clamp(tile.CurrentHitPoints - damage, 0, tileDef.MaxHitPoints);
                _grid[x, y].CurrentHitPoints = currentHitPoints;
                if (currentHitPoints <= 0)
                {
                    TrySetTileAt(x, y, Map.Tile.None, true);
                }
            }
        }

        public bool HasTileAt(int x, int y)
        {
            return IsWithinBounds(x, y, 0, _mapMetadata.Dimensions) && _grid[x, y] != TileInstance.None;
        }

        public bool IsTileLoaded(int x, int y)
        {
            if (_mapMetadata == null)
            {
                return false;
            }

            return IsWithinBounds(x, y, 0, _mapMetadata.Dimensions); // load from chunk
        }

        public bool TrySetTileAt(int x, int y, Core.Map.Tile tile, bool overrideTile = false)
        {
            if (!overrideTile && HasTileAt(x, y))
            {
                return false;
            }

            var tileDefinition = _tilesSettings.GetDefinition(tile);
            var tileInstance = (TileInstance)tileDefinition;
            var tileBase = _tilesSettings.GetTileBase(tile);

            _grid[x, y] = tileInstance;
            _tilemap.SetTile(new Vector3Int(x, y, 0), tileBase);

            TileChanged?.Invoke(x, y, tileInstance, tileDefinition);

            return true;
        }

        protected override void OnInitialize()
        {
            ServiceLocator.Set<IGridService>(this);
            _gridDrawer = new(this, _gridSpriteRendererPrefab);
            MapMetadataGeneratedEvent.AddListener(MapLoadedEventHandler);
        }

        protected override void OnSpawn()
        {
            _tilesSettings = ScriptableSettings.GetOrFind<TilesSettings>();
        }

        private void MapLoadedEventHandler(ref EventContext context, in MapMetadataGeneratedEvent e)
        {
            InitializeGrid(e.MapMetadata);

            _mapMetadata = e.MapMetadata;

            if (_useChunkLoad)
            {
                _chunkController = new(
                    _mapMetadata,
                    _chunkSize,
                    _loadNearChunks,
                    _positionEventBus,
                    this,
                    _tilesSettings);
            }
            else
            {
                LoadFullTilemap();
            }

            Initialized?.Invoke();
        }

        private void InitializeGrid(MapMetadata mapMetadata)
        {
            _gridSize = mapMetadata.Dimensions;
            _grid = new TileInstance[_gridSize, _gridSize];

            for (int i = 0; i < mapMetadata.Dimensions; i++)
            {
                for (int j = 0; j < mapMetadata.Dimensions; j++)
                {
                    var td = _tilesSettings.GetDefinition(mapMetadata.Tiles[i, j]);
                    var ti = (TileInstance)td;
                    _grid[i, j] = ti;
                }
            }
        }

        public void LoadGridBlock(int dimensions, Core.Map.Tile[,] tiles)
        {
            for (int x = 0; x < dimensions; x++)
            {
                for (int y = 0; y < dimensions; y++)
                {
                    var tile = tiles[x, y];
                    var tileInstance = (TileInstance)_tilesSettings.GetDefinition(tile);
                    _grid[x, y] = tileInstance;
                }
            }
        }

        public void LoadTilesBlock(BoundsInt area, TileBase[] tiles)
        {
            _tilemap.SetTilesBlock(area, tiles);
        }

        private void LoadFullTilemap()
        {
            int dimensions = _mapMetadata.Dimensions;
            var fullArea = new BoundsInt(0, 0, 0, dimensions, dimensions, 1);

            var allTiles = new TileBase[dimensions * dimensions];
            var allFloorTiles = new TileBase[dimensions * dimensions];

            for (int y = 0; y < dimensions; y++)
            {
                for (int x = 0; x < dimensions; x++)
                {
                    int index = y * dimensions + x;
                    allTiles[index] = _tilesSettings.GetTileBase(_mapMetadata.Tiles[x, y]);
                    allFloorTiles[index] = _tilesSettings.GetFloorTileBase(_mapMetadata.BiomeTiles[x, y]);
                }
            }

            LoadTilesBlock(fullArea, allTiles);
            LoadFloorTilesBlock(fullArea, allFloorTiles);
        }

        public void LoadFloorTilesBlock(BoundsInt area, TileBase[] tiles)
        {
            _floorTilemap.SetTilesBlock(area, tiles);
        }

        public void LateUpdate()
        {
            _gridDrawer.Clear();
        }
    }
}