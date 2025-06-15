using Coimbra.Services;
using Core.Debugger;
using Core.Map;
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
        private readonly FungusDefinition _foodDef;

        private readonly List<Vector2Int> _keysToUpdate = new();

        public object GetData(Vector2Int vector)
        {
            var data = _dataMap[vector];
            return new FungusTileData()
            {
                CurrentHealth = data.CurrentHealth,
                CurrentSaciation = data.CurrentSaciation,
                CurrentFoodStore = data.CurrentFoodStore,
                FoodProduction = _foodDef.FoodProduction,
                LostHealthWhenStarved = _foodDef.LostHealthWhenStarved,
                MaxFoodStore = _foodDef.MaxFoodStore,
                MaxHealth = _foodDef.MaxHealth,
                SaciationLost = _foodDef.SaciationLost
            };
        }

        public bool TryGetData(Vector2Int position, out FungusData data)
        {
            return _dataMap.TryGetValue(position, out data);
        }

        public bool TryTakeFungusFood(Vector2Int position)
        {
            if (_dataMap.TryGetValue(position, out FungusData data) && data.CurrentFoodStore >= 5f)
            {
                var modifiedData = data;
                modifiedData.CurrentFoodStore -= 5f;
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

        public FungusRunner(IGridService grid, IChemicalGridService chemicals)
        {
            ServiceLocator.Set<IFungusService>(this);

            _gridService = grid;
            _chemicalService = chemicals;

            _foodDef = new()
            {
                FoodProduction = 0.001f,
                LostHealthWhenStarved = 0.001f,
                SaciationLost = 0.001f,
                MaxHealth = 50f,
                MaxFoodStore = 50f,
                MaxSaciation = 50f,
            };

            for (int i = 0; i < grid.Dimensions; i++)
            {
                for (int j = 0; j < grid.Dimensions; j++)
                {
                    if (grid.Grid[i, j].TileType == Tile.Fungus)
                    {
                        HandleTileChanged(i, j, grid.Grid[i, j]);
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
                    CurrentHealth = _foodDef.MaxHealth,
                    CurrentFoodStore = 0,
                    CurrentSaciation = _foodDef.MaxSaciation
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
                _logic.OnUpdate(ref data, _foodDef, position, _gridService, _chemicalService);
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
        bool TryApplyModification(Vector2Int position, ModifyFungusData modification);
        public bool TryTakeFungusFood(Vector2Int position);
        bool TryGetData(Vector2Int position, out FungusData data);
    }

    public delegate void ModifyFungusData(ref FungusData data);
}