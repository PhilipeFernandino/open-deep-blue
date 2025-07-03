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
    public class FungusRunner : ILogicRunner, IFungusService
    {
        private readonly Dictionary<Vector2Int, FungusData> _dataMap = new();
        private readonly FungusLogic _logic = new FungusLogic();
        private readonly IGridService _gridService;
        private readonly IChemicalGridService _chemicalService;
        private readonly FungusDefinition _fungusDef;

        private readonly List<Vector2Int> _keysToUpdate = new();

        private float _gatherCost;

        public object GetData(Vector2Int vector)
        {
            var data = _dataMap[vector];
            return new FungusTileData()
            {
                CurrentHealth = data.CurrentHealth,
                CurrentSaciation = data.CurrentSaciation,
                CurrentFoodStore = data.CurrentFoodStore,
                FoodProduction = _fungusDef.FoodProduction,
                LostHealthWhenStarved = _fungusDef.LostHealthWhenStarved,
                MaxFoodStore = _fungusDef.MaxFoodStore,
                MaxHealth = _fungusDef.MaxHealth,
                SaciationLost = _fungusDef.SaciationLost
            };
        }

        public (Vector2Int position, FungusData data) GetAny()
        {
            foreach (var pair in _dataMap)
                return (pair.Key, pair.Value);

            return default;
        }


        public bool TryGetData(Vector2Int position, out FungusData data)
        {
            return _dataMap.TryGetValue(position, out data);
        }

        public bool TryTakeFungusFood(Vector2Int position)
        {
            if (_dataMap.TryGetValue(position, out FungusData data) && data.CurrentFoodStore >= _gatherCost)
            {
                var modifiedData = data;
                modifiedData.CurrentFoodStore -= _gatherCost;
                _dataMap[position] = modifiedData;
                return true;
            }
            return false;
        }

        public bool TryApplyModification(Vector2Int position, ModifyFungusData modificationAction)
        {
            if (_dataMap.TryGetValue(position, out FungusData data))
            {
                var modifiedData = data;
                modificationAction(ref modifiedData);
                _dataMap[position] = modifiedData;
                return true;
            }
            return false;
        }

        public FungusRunner(ScriptableObject fungusDef)
        {
            ServiceLocator.Set<IFungusService>(this);

            _gridService = ServiceLocatorUtilities.GetServiceAssert<IGridService>();
            _chemicalService = ServiceLocatorUtilities.GetServiceAssert<IChemicalGridService>();
            _gatherCost = ScriptableSettings.GetOrFind<ColonyEconomySettings>().FungusFeedAntsAmount;

            _fungusDef = (FungusDefinition)fungusDef;

            for (int i = 0; i < _gridService.Dimensions; i++)
            {
                for (int j = 0; j < _gridService.Dimensions; j++)
                {
                    if (_gridService.Grid[i, j].TileType == Tile.Fungus)
                    {
                        HandleTileChanged(i, j, _gridService.Grid[i, j]);
                    }
                }
            }
        }

        public void HandleTileChanged(int x, int y, TileInstance newTile)
        {
            var position = new Vector2Int(x, y);
            if (newTile.TileType == Tile.Fungus)
            {
                _dataMap[position] = new FungusData
                {
                    CurrentHealth = _fungusDef.MaxHealth,
                    CurrentFoodStore = 0,
                    CurrentSaciation = _fungusDef.MaxSaciation
                };
            }
            else
            {
                _dataMap.Remove(position);
            }
        }

        public void FixedUpdate()
        {
            _keysToUpdate.Clear();

            foreach (var key in _dataMap.Keys)
            {
                _keysToUpdate.Add(key);
            }

            foreach (var position in _keysToUpdate)
            {
                var data = _dataMap[position];
                _logic.OnUpdate(ref data, _fungusDef, position, _gridService, _chemicalService);
                _dataMap[position] = data;
            }
        }

        public void Dispose()
        {

        }
    }

    [DynamicService]
    public interface IFungusService : IService
    {
        public (Vector2Int position, FungusData data) GetAny();
        bool TryApplyModification(Vector2Int position, ModifyFungusData modification);
        public bool TryTakeFungusFood(Vector2Int position);
        bool TryGetData(Vector2Int position, out FungusData data);
    }

    public delegate void ModifyFungusData(ref FungusData data);
}