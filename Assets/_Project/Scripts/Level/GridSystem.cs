using Coimbra;
using Coimbra.Services;
using Coimbra.Services.Events;
using Core.EventBus;
using Core.Map;
using NaughtyAttributes;
using System.Text;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;
using UnityEngine.WSA;
using static Core.Util.Range;

namespace Core.Level
{
    public class GridSystem : Actor, IGridService
    {
        [Tooltip("Size of the grid from -thisValue to thisValue")]
        [SerializeField] private int _spritesPoolSize;
        [SerializeField] private Tilemap _tilemap;
        [SerializeField] private Tilemap _floorTilemap;
        [SerializeField] private SpriteRenderer _gridSpriteRendererPrefab;
        [SerializeField] private bool _loadOnStart;

        [Header("Chunk Control")]
        [SerializeField] private int _chunkSize;
        [SerializeField] private int _loadNearChunks;
        [SerializeField] private PositionEventBus _positionEventBus;

        private TileInstance[,] _grid;
        private TileChunkController _chunkController;
        private TilesSettings _tilesSettings;
        private GridDrawer _gridDrawer;

        private int _gridSize;
        private bool _isInitialized = false;

        private TileInstance[,] _underworld;
        private TileInstance[,] _overworld;

        private MapMetadata _mapMetadata;

        public int ChunkSize => _chunkSize;
        public int LoadedDimensions => _chunkSize * (_loadNearChunks * 2 + 1);
        public int MapDimensions => _mapMetadata.Dimensions;

        public void ClearDrawAt(Vector2 position) => _gridDrawer?.ClearAt(position);
        public void DrawInGrid(Vector2 position, Color color) => _gridDrawer?.DrawInGrid(position, color);
        public void DrawInGrid(Vector2 position, in Vector2Int size, Color color) => _gridDrawer?.DrawInGrid(position, size, color);

        public void Update()
        {
            var mousePos = Mouse.current.position.ReadValue();
            var pos = Camera.main.ScreenToWorldPoint(mousePos);
            pos.z = 0f;

            _gridDrawer.DrawInGrid(pos, Color.white);

            Vector3Int v3 = new((int)pos.x, (int)pos.y, (int)pos.z);

            Debug.Log($"Tile at {pos} - {GetTileAt(pos).TileType} - {_tilemap.GetTile(v3)}");
        }

        public Map.TileInstance GetTileAt(Vector3 pos)
        {
            var v2 = Vector2Int.FloorToInt(pos.XY());
            if (IsWithinBounds(v2.x, v2.y, 0, _gridSize))
                return _grid[v2.x, v2.y];
            return Map.TileInstance.None;
        }

        public bool TryGetTileAt(int x, int y, out TileInstance tile)
        {
            if (!IsWithinBounds(x, y, 0, _mapMetadata.Dimensions))
            {
                tile = TileInstance.None;
                return false;
            }

            tile = _grid[x, y];
            Debug.Log($"{_grid[x, y]}, {_mapMetadata.Tiles[x, y]}");
            return true;
        }

        public void DamageTileAt(int x, int y, int damage)
        {
            Debug.Log($"{GetType()} - DamageTileAt - {x}, {y}, {damage}");
            int currentHitPoints = Mathf.Max(0, (int)_grid[x, y].CurrentHitPoints - (int)damage);
            if (currentHitPoints <= 0)
            {
                TrySetTileAt(x, y, Map.Tile.None);
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

            var tileInstance = (TileInstance)_tilesSettings.GetDefinition(tile);
            var tileBase = _tilesSettings.GetTileBase(tile);

            _grid[x, y] = tileInstance;
            _tilemap.SetTile(new Vector3Int(x, y, 0), tileBase);

            return true;
        }

        protected override void OnInitialize()
        {
            ServiceLocator.Set<IGridService>(this);
            _gridDrawer = new(this, _gridSpriteRendererPrefab);
            MapMetadataGeneratedEvent.AddListener(MapLoaded_EventHandler);
        }

        protected override void OnSpawn()
        {
            _tilesSettings = ScriptableSettings.GetOrFind<TilesSettings>();
        }

        private void MapLoaded_EventHandler(ref EventContext context, in MapMetadataGeneratedEvent e)
        {
            Debug.Log($"{GetType()} - {nameof(MapLoaded_EventHandler)}");

            InitializeGrid(e.MapMetadata);
            _isInitialized = true;
            _mapMetadata = e.MapMetadata;
            _chunkController = new(
                _mapMetadata,
                _chunkSize,
                _loadNearChunks,
                _positionEventBus,
                this,
                _tilesSettings);


            if (_loadOnStart)
            {
                _chunkController.UpdatePosition(_positionEventBus.Position);
            }
        }

        private void InitializeGrid(MapMetadata mapMetadata)
        {
            _gridSize = mapMetadata.Dimensions;
            _grid = new TileInstance[_gridSize, _gridSize];


            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < mapMetadata.Dimensions; i++)
            {
                for (int j = 0; j < mapMetadata.Dimensions; j++)
                {
                    var td = _tilesSettings.GetDefinition(mapMetadata.Tiles[i, j]);
                    var ti = (TileInstance)td;
                    _grid[i, j] = ti;
                    if (_grid[i, j].TileType == Map.Tile.None && mapMetadata.Tiles[i, j] != Map.Tile.None)
                    {
                        throw new System.Exception($"{td} and ${ti}");

                    }
                    sb.Append($"({i}, {j}, {mapMetadata.Tiles[i, j]}, {_grid[i, j].TileType})");
                }
            }
            Debug.Log(sb);
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
            Debug.Log($"Loaded tiles block area: {area}");
            _tilemap.SetTilesBlock(area, tiles);
        }

        public void LoadFloorTilesBlock(BoundsInt area, TileBase[] tiles)
        {
            _floorTilemap.SetTilesBlock(area, tiles);
        }

        public void LateUpdate()
        {
            _gridDrawer.Clear();
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