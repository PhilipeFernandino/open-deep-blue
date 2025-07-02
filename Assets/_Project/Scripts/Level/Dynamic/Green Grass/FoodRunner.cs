using Coimbra;
using Coimbra.Services;
using Core.Debugger;
using Core.Map;
using Core.Train;
using Core.Util;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Level.Dynamic
{
    public class FoodRunner : ILogicRunner, IFoodService
    {
        private readonly Dictionary<Vector2Int, FoodData> _dataMap = new();
        private readonly FoodLogic _logic = new();
        private readonly IGridService _gridService;
        private readonly IChemicalGridService _chemicalService;
        private readonly FoodDefinition _foodDef;

        private readonly List<Vector2Int> _keysToUpdate = new();

        private float _eatCost;

        public object GetData(Vector2Int vector)
        {
            var data = _dataMap[vector];
            return new FoodTileData()
            {
                CurrentFoodStore = data.CurrentFoodStore,
                MaxFoodStore = _foodDef.MaxFoodStore,
            };
        }

        public (Vector2Int position, FoodData foodData, FoodDefinition foodDefinition) GetAny()
        {
            foreach (var pair in _dataMap)
                return (pair.Key, pair.Value, _foodDef);

            return default;
        }


        public bool TryGetData(Vector2Int position, out FoodData data)
        {
            return _dataMap.TryGetValue(position, out data);
        }

        public bool TryEat(Vector2Int position)
        {
            if (_dataMap.TryGetValue(position, out FoodData data))
            {
                var modifiedData = data;
                modifiedData.CurrentFoodStore -= _eatCost;
                _dataMap[position] = modifiedData;
                if (data.CurrentFoodStore <= 0)
                {
                    _gridService.TrySetTileAt(position, Map.Tile.None, true);
                }
                return true;
            }
            return false;
        }

        public FoodRunner(ScriptableObject foodDef)
        {
            ServiceLocator.Set<IFoodService>(this);

            _gridService = ServiceLocatorUtilities.GetServiceAssert<IGridService>();
            _chemicalService = ServiceLocatorUtilities.GetServiceAssert<IChemicalGridService>();

            _eatCost = ScriptableSettings.GetOrFind<ColonyEconomySettings>().LeafFeedFungusAmount;

            _foodDef = (FoodDefinition)foodDef;

            for (int i = 0; i < _gridService.Dimensions; i++)
            {
                for (int j = 0; j < _gridService.Dimensions; j++)
                {

                    HandleTileChanged(i, j, _gridService.Grid[i, j]);
                }
            }
        }

        public void HandleTileChanged(int x, int y, TileInstance newTile)
        {
            var position = new Vector2Int(x, y);
            if (newTile.TileType == Tile.GreenGrass)
            {
                _dataMap[position] = new FoodData
                {
                    CurrentFoodStore = _foodDef.MaxFoodStore,
                };
            }
            else
            {
                _dataMap.Remove(position);
            }

            _keysToUpdate.Clear();

            foreach (var key in _dataMap.Keys)
            {
                _keysToUpdate.Add(key);
            }
        }

        public void FixedUpdate()
        {
            foreach (var position in _keysToUpdate)
            {
                var data = _dataMap[position];
                _logic.OnUpdate(ref data, _foodDef, position, _gridService);
                _dataMap[position] = data;
            }
        }

        public void Dispose()
        {

        }
    }

    [DynamicService]
    public interface IFoodService : IService
    {
        public (Vector2Int position, FoodData foodData, FoodDefinition foodDefinition) GetAny();
        public bool TryEat(Vector2Int position);
        bool TryGetData(Vector2Int position, out FoodData data);
    }
}