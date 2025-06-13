using Coimbra.Services;
using Core.Debugger;
using Core.Map;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Level.Dynamic
{
    public class QueenRunner : ILogicRunner, IQueenService
    {
        private readonly Dictionary<Vector2Int, QueenData> _dataMap = new();
        private readonly QueenLogic _logic = new QueenLogic();
        private readonly IGridService _gridService;
        private readonly IChemicalGridService _chemicalService;
        private readonly QueenDefinition _queenDef;

        private readonly List<Vector2Int> _keysToUpdate = new();

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

        public QueenRunner(IGridService grid, IChemicalGridService chemicals)
        {
            _gridService = grid;
            _chemicalService = chemicals;

            _queenDef = new()
            {
                LostHealthWhenStarved = 0.001f,
                SaciationLost = 0.001f,
                MaxHealth = 50f,
                MaxSaciation = 50f,
                BroodPerLaying = 3,
                PregnancyRate = 0.001f,
            };

            for (int i = 0; i < grid.MapDimensions; i++)
            {
                for (int j = 0; j < grid.MapDimensions; j++)
                {
                    if (grid.Grid[i, j].TileType == Tile.QueenAnt)
                    {
                        HandleTileChanged(i, j, grid.Grid[i, j]);
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
                _logic.OnUpdate(ref data, _queenDef, position, _gridService, _chemicalService);
                _dataMap[position] = data;
                _chemicalService.Drop(position, Chemical.QueenPheromone, 150f * Time.deltaTime);
            }
        }

        public void Dispose()
        {
        }
    }

    [DynamicService]
    public interface IQueenService : IService
    {
        public bool TryFeedTheQueen(Vector2Int position, float foodAmount);
    }
}