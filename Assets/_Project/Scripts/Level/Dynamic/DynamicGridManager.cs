using Coimbra;
using Coimbra.Services;
using Core.Debugger;
using Core.Map;
using Core.Util;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Core.Level.Dynamic
{
    [Serializable]
    public class TileToDataDef
    {
        public Tile Tile;
        public ScriptableObject Definition;
    }

    public class DynamicGridManager : Actor, IDynamicGridManager
    {
        [Header("Debugging")]
        [SerializeField] private DebugChannelSO _debugChannel;
        [SerializeField] private bool _debug = true;
        [SerializeField] private List<TileToDataDef> _tileToDataDef;

        private readonly Dictionary<Tile, ILogicRunner> _runnerMap = new();

        private IGridService _gridService;

        protected override void OnSpawn()
        {
            _gridService = ServiceLocatorUtilities.GetServiceAssert<IGridService>();
            _gridService.Initialized += Setup;
        }

        private ScriptableObject DataDefinition(Tile tile)
        {
            return _tileToDataDef.Find((t) => t.Tile == tile).Definition;
        }

        private void Setup()
        {
            var fungusRunner = new FungusRunner(DataDefinition(Tile.Fungus));
            _runnerMap[Tile.Fungus] = fungusRunner;

            var antQueenRunner = new QueenRunner(DataDefinition(Tile.QueenAnt));
            _runnerMap[Tile.QueenAnt] = antQueenRunner;

            var foodRunner = new FoodRunner(DataDefinition(Tile.GreenGrass));
            _runnerMap[Tile.GreenGrass] = foodRunner;

            _gridService.TileChanged += HandleTileChanged;
        }

        private void HandleTileChanged(int x, int y, TileInstance newTile, TileDefinition tileDefinition)
        {
            if (_runnerMap.TryGetValue(newTile.TileType, out var runner))
            {
                runner.HandleTileChanged(x, y, newTile);
            }
        }

        private void FixedUpdate()
        {
            foreach (var runner in _runnerMap.Values)
            {
                runner.FixedUpdate();
            }
        }

        private void Update()
        {
            Debug();
        }

        private void Debug()
        {
            if (!_debug)
                return;

            var mousePos = Mouse.current.position.ReadValue();
            var pos = Camera.main.ScreenToWorldPoint(mousePos);
            pos.z = 0f;

            var (x, y) = IGridService.ToGridPosition(pos);
            _gridService.TryGetTileAt(x, y, out var tileInstance);

            if (tileInstance.TileType == Tile.Fungus)
            {
                var data = (FungusTileData)_runnerMap[Tile.Fungus].GetData(x, y);
                _debugChannel.RaiseEvent("dynamic", data);
            }
            else if (tileInstance.TileType == Tile.QueenAnt)
            {
                var data = (QueenTileData)_runnerMap[Tile.QueenAnt].GetData(x, y);
                _debugChannel.RaiseEvent("dynamic", data);
            }
            else if (tileInstance.TileType == Tile.GreenGrass)
            {
                var data = (FoodTileData)_runnerMap[Tile.GreenGrass].GetData(x, y);
                _debugChannel.RaiseEvent("dynamic", data);
            }
        }

    }

    [DynamicService]
    public interface IDynamicGridManager : IService
    {
    }
}