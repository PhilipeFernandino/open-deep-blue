using Coimbra;
using Coimbra.Services;
using Core.Debugger;
using Core.Map;
using Core.Util;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;
using UnityEngine.UIElements;

namespace Core.Level.Dynamic
{
    public class DynamicGridManager : Actor, IDynamicGridManager
    {
        [Header("Debugging")]
        [SerializeField] private DebugChannelSO _debugChannel;
        [SerializeField] private bool _debug = true;

        private readonly Dictionary<Tile, ILogicRunner> _runnerMap = new();

        private IGridService _gridService;
        private IChemicalGridService _chemicalService;
        private TilesSettings _tilesSettings;

        public bool TryGetFungusFood(Vector2Int pos)
        {
            if (_runnerMap.TryGetValue(Tile.Fungus, out var runner) && runner is FungusRunner fungusRunner)
            {
                return fungusRunner.TryGetFungusFood(pos);
            }
            return false;
        }

        public bool TryApplyFungusModification(Vector2Int position, ModifyFungusData modification)
        {
            if (_runnerMap.TryGetValue(Tile.Fungus, out var runner) && runner is FungusRunner fungusRunner)
            {
                return fungusRunner.TryApplyModification(position, modification);
            }
            return false;
        }

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

    [DynamicService]
    public interface IDynamicGridManager : IService
    {
        public bool TryGetFungusFood(Vector2Int position);
        public bool TryApplyFungusModification(Vector2Int position, ModifyFungusData modification);
    }

    public delegate void ModifyFungusData(ref FungusData data);

}