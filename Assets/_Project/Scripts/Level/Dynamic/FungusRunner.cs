using Core.Debugger;
using Core.Map;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Core.Level.Dynamic
{
    public class FungusRunner : ILogicRunner
    {
        private readonly Dictionary<Vector2Int, FungusData> _dataMap = new();
        private readonly FungusLogic _logic = new FungusLogic();
        private readonly IGridService _gridService;
        private readonly IChemicalGridService _chemicalService;
        private readonly TileDefinition _tileDefinition;
        private readonly FungusDefinition _fungusDef;

        private readonly List<Vector2Int> _keysToUpdate = new();

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

        public bool TryGetFungusFood(Vector2Int position)
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

        public FungusRunner(IGridService grid, IChemicalGridService chemicals, TileDefinition definition)
        {
            _gridService = grid;
            _chemicalService = chemicals;
            _tileDefinition = definition;

            _fungusDef = new()
            {
                FoodProduction = 0.001f,
                LostHealthWhenStarved = 0.001f,
                SaciationLost = 0.001f,
                MaxHealth = 50f,
                MaxFoodStore = 50f,
                MaxSaciation = 50f,
            };

            for (int i = 0; i < grid.MapDimensions; i++)
            {
                for (int j = 0; j < grid.MapDimensions; j++)
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
            if (newTile.TileType == _tileDefinition.TileType)
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
                _chemicalService.Drop(position, Chemical.FungusScent, 100f);
            }
        }
    }
}