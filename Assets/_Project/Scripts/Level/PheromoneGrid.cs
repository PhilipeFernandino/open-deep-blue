using Coimbra;
using Coimbra.Services;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Level
{
    public class PheromoneGrid : Actor, IPheromoneService
    {
        [SerializeField] private int _dimensions;

        private Dictionary<AntPheromone, PheromoneMap> _pheromoneGrid = new();

        public object this[AntPheromone i]
        {
            get { return _pheromoneGrid[i]; }
        }

        public bool TryGet(AntPheromone antPheromone, out PheromoneMap value)
        {
            return _pheromoneGrid.TryGetValue(antPheromone, out value);
        }

        public void Drop(int x, int y, AntPheromone antPheromone, int value)
        {
            _pheromoneGrid[antPheromone].Sum(x, y, value);
        }

        public void Remove(int x, int y, AntPheromone antPheromone, int value)
        {
            _pheromoneGrid[antPheromone].Sum(x, y, -value);
        }

        public void Clean(int x, int y, AntPheromone antPheromone)
        {
            _pheromoneGrid[antPheromone].Set(x, y, 0);
        }

        protected override void OnSpawn()
        {
            ServiceLocator.Set<IPheromoneService>(this);
        }

        protected override void OnInitialize()
        {
            foreach (var x in Enum.GetValues(typeof(AntPheromone)))
            {
                _pheromoneGrid.Add((AntPheromone)x, new(_dimensions));
            }
        }
    }

    [DynamicService]
    public interface IPheromoneService : IService
    {
        public object this[AntPheromone i]
        {
            get;
        }

        public bool TryGet(AntPheromone antPheromone, out PheromoneMap value);

        public void Drop(int x, int y, AntPheromone antPheromone, int value);

        public void Remove(int x, int y, AntPheromone antPheromone, int value);

        public void Clean(int x, int y, AntPheromone antPheromone);

        public void Drop(Vector2 v, AntPheromone antPheromone, int value)
        {
            var (x, y) = ToGridPosition(v);
            Drop(x, y, antPheromone, value);
        }

        public void Remove(Vector2 v, AntPheromone antPheromone, int value)
        {
            var (x, y) = ToGridPosition(v);
            Remove(x, y, antPheromone, value);
        }

        public void Clean(Vector2 v, AntPheromone antPheromone)
        {
            var (x, y) = ToGridPosition(v);
            Clean(x, y, antPheromone);
        }

        public static (int x, int y) ToGridPosition(Vector2 v)
        {
            Vector2Int v2 = new(Mathf.RoundToInt(v.x), Mathf.RoundToInt(v.y));
            return (v2.x, v2.y);
        }

    }
}