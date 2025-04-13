using Coimbra;
using Coimbra.Services;
using Coimbra.Services.Events;
using Core.EventBus;
using Core.Map;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.InputSystem;
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

        private TileInstance[,] _grid;
        private TileChunkController _chunkController;
        private TilesSettings _tilesSettings;
        private GridDrawer _gridDrawer;

        private int _gridSize;
        private int _lastGridDrawSize = 0;
        private bool _isInitialized = false;


        private MapMetadata _mapMetadata;

        public int ChunkSize => _chunkSize;
        public int LoadedDimensions => _chunkSize * (_loadNearChunks * 2 + 1);
        public int MapDimensions => _mapMetadata.Dimensions;

        public void ClearDrawAt(Vector2 position) => _gridDrawer.ClearAt(position);
        public void DrawInGrid(Vector2 position, Color color) => _gridDrawer.DrawInGrid(position, color);
        public void DrawInGrid(Vector2 position, in Vector2Int size, Color color) => _gridDrawer.DrawInGrid(position, size, color);

        public void Update()
        {
            var mousePos = Mouse.current.position.ReadValue();
            var pos = Camera.main.ScreenToWorldPoint(mousePos);
            pos.z = 0f;

            _gridDrawer.DrawInGrid(pos, Color.white);
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

        public void DamageTileAt(int x, int y, ushort damage)
        {
            Debug.Log($"{GetType()} - DamageTileAt - {x}, {y}, {damage}");
            int currentHitPoints = Mathf.Max(0, (int)_grid[x, y].CurrentHitPoints - (int)damage);
            if (currentHitPoints <= 0)
            {
                SetTileAt(x, y, Map.Tile.None);
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

        public void SetTileAt(int x, int y, Core.Map.Tile tile)
        {
            var tileInstance = (TileInstance)_tilesSettings.GetDefinition(tile);
            var tileBase = _tilesSettings.GetTileBase(tile);

            _grid[x, y] = tileInstance;
            _tilemap.SetTile(new Vector3Int(x, y, 0), tileBase);
        }

        protected override void OnInitialize()
        {
            ServiceLocator.Set<IGridService>(this);
            _gridDrawer = new(this, _gridSpriteRendererPrefab);
        }

        protected override void OnSpawn()
        {
            _tilesSettings = ScriptableSettings.GetOrFind<TilesSettings>();
            MapMetadataGeneratedEvent.AddListener(MapLoaded_EventHandler);
        }

        private void MapLoaded_EventHandler(ref EventContext context, in MapMetadataGeneratedEvent e)
        {
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

        public void LoadTilesBlock(BoundsInt area, TileBase[] tiles)
        {
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