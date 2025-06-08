using Coimbra;
using Core.Debugger;
using Core.Map;
using Core.Util;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Core.Level.Dynamic
{
    public class DynamicTileManager : Actor
    {
        [Header("Debugging")]
        [SerializeField] private DebugChannelSO _debugChannel;
        [SerializeField] private bool _debug = true;

        private readonly Dictionary<Tile, ILogicRunner> _runnerMap = new();

        private IGridService _gridService;
        private IChemicalGridService _chemicalService;
        private TilesSettings _tilesSettings;

        protected override void OnSpawn()
        {
            _gridService = ServiceLocatorUtilities.GetServiceAssert<IGridService>();
            _chemicalService = ServiceLocatorUtilities.GetServiceAssert<IChemicalGridService>();
            _tilesSettings = ScriptableSettings.GetOrFind<TilesSettings>();
            _gridService.Initialized += Setup;
        }

        private void Setup()
        {
            var definition = _tilesSettings.GetDefinition(Tile.Fungus);

            var runner = new FungusRunner(_gridService, _chemicalService, definition);
            _runnerMap[definition.TileType] = runner;

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
        }

    }
}