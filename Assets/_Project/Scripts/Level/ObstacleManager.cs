using Coimbra;
using Coimbra.Services;
using Core.Map;
using Core.Util;
using System;
using Unity.Collections;

namespace Core.Level
{

    [DynamicService]
    public interface IObstacleService : IService
    {
        public bool HasObstacle(int x, int y);
        public NativeArray<bool>.ReadOnly GetObstacleGrid();
        public int Dimensions { get; }
    }

    public class ObstacleManager : Actor, IObstacleService
    {
        private IGridService _gridService;
        private NativeArray<bool> _obstacleGrid;
        private TilesSettings _tilesSettings;

        private bool _isInitialized = false;

        public int Dimensions => _gridService?.Dimensions ?? 0;

        public NativeArray<bool>.ReadOnly GetObstacleGrid() => _obstacleGrid.AsReadOnly();

        public bool HasObstacle(int x, int y)
        {
            if (!Util.Range.IsWithinBounds(x, y, 0, Dimensions))
            {
                return true;
            }

            int index = y * Dimensions + x;
            return _obstacleGrid[index];
        }

        protected override void OnInitialize()
        {
            ServiceLocator.Set<IObstacleService>(this);
        }

        protected override void OnSpawn()
        {
            _gridService = ServiceLocatorUtilities.GetServiceAssert<IGridService>();
            _tilesSettings = ScriptableSettings.GetOrFind<TilesSettings>();
            _gridService.Initialized += Setup;
        }

        protected override void OnDestroyed()
        {
            if (_gridService != null)
            {
                _gridService.Initialized -= Setup;
                _gridService.TileChanged -= HandleTileChanged;
            }

            if (_obstacleGrid.IsCreated)
            {
                _obstacleGrid.Dispose();
            }
        }

        private void Setup()
        {
            OnDestroyed();

            int dimensions = _gridService.Dimensions;
            _obstacleGrid = new NativeArray<bool>(dimensions * dimensions, Allocator.Persistent);

            BuildInitialObstacleMap();
            _gridService.TileChanged += HandleTileChanged;
            _isInitialized = true;
        }

        private void BuildInitialObstacleMap()
        {
            var grid = _gridService.Grid;
            int dimensions = _gridService.Dimensions;
            for (int x = 0; x < dimensions; x++)
            {
                for (int y = 0; y < dimensions; y++)
                {
                    _obstacleGrid[y * dimensions + x] = !_tilesSettings.GetDefinition(grid[x, y].TileType).IsWalkable;
                }
            }
        }

        private void HandleTileChanged(int x, int y, TileInstance tile, TileDefinition tileDef)
        {
            _obstacleGrid[y * Dimensions + x] = !tileDef.IsWalkable;
        }
    }
}