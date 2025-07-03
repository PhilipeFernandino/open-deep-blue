using Coimbra.Services;
using Core.Debugger;
using Core.Map;
using Core.Util;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Level.Dynamic
{
    public class QueenRunner : ILogicRunner, IQueenService
    {
        private readonly Dictionary<Vector2Int, QueenData> _dataMap = new();
        private readonly QueenLogic _logic = new();
        private readonly IGridService _gridService;
        private readonly IChemicalGridService _chemicalService;
        private readonly QueenDefinition _queenDef;

        private readonly List<Vector2Int> _keysToUpdate = new();

        public (Vector2Int position, QueenData data, QueenDefinition queenDefinition) GetAny()
        {
            foreach (var pair in _dataMap)
                return (pair.Key, pair.Value, _queenDef);

            return default;
        }

        public object GetData(Vector2Int vector)
        {
            var data = _dataMap[vector];
            return new QueenTileData()
            {
                QueenData = data,
                QueenDefinition = _queenDef
            };
        }

        public bool TryFeedTheQueen(Vector2Int position, float foodAmount)
        {
            if (_dataMap.TryGetValue(position, out QueenData data))
            {
                var modifiedData = data;
                modifiedData.CurrentSaciation = Mathf.Min(foodAmount + modifiedData.CurrentSaciation, _queenDef.MaxSaciation);
                _dataMap[position] = modifiedData;
                return true;
            }
            return false;
        }

        public QueenRunner(ScriptableObject queenDef)
        {
            _gridService = ServiceLocatorUtilities.GetServiceAssert<IGridService>();
            _chemicalService = ServiceLocatorUtilities.GetServiceAssert<IChemicalGridService>();

            _queenDef = (QueenDefinition)queenDef;

            for (int i = 0; i < _gridService.Dimensions; i++)
            {
                for (int j = 0; j < _gridService.Dimensions; j++)
                {
                    if (_gridService.Grid[i, j].TileType == Tile.QueenAnt)
                    {
                        HandleTileChanged(i, j, _gridService.Grid[i, j]);
                    }
                }
            }

            ServiceLocator.Set<IQueenService>(this);
        }

        public void HandleTileChanged(int x, int y, TileInstance newTile)
        {
            var position = new Vector2Int(x, y);
            if (newTile.TileType == Tile.QueenAnt)
            {
                _dataMap[position] = new QueenData
                {
                    CurrentHealth = _queenDef.MaxHealth,
                    CurrentPregnancyPercentage = 0f,
                    CurrentSaciation = _queenDef.MaxSaciation
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
                _logic.OnUpdate(ref data, _queenDef, position, _chemicalService);
                _dataMap[position] = data;
            }
        }

        public void Dispose()
        {
        }
    }

    [DynamicService]
    public interface IQueenService : IService
    {
        public (Vector2Int position, QueenData data, QueenDefinition queenDefinition) GetAny();
        public bool TryFeedTheQueen(Vector2Int position, float foodAmount);
    }
}