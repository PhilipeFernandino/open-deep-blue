using UnityEngine;

namespace Core.Level
{
    public class PheromoneMap
    {
        public int[,] _pheromone;

        private int _dimensions;

        public PheromoneMap(int dimensions)
        {
            _dimensions = dimensions;
            _pheromone = new int[dimensions, dimensions];
        }

        public void SumAll(int value)
        {
            for (int i = 0; i < _dimensions; i++)
            {
                for (int j = 0; j < _dimensions; j++)
                {
                    _pheromone[i, j] = Mathf.Max(_pheromone[i, j] - value, 0);
                }
            }
        }

        public void Sum(int x, int y, int value)
        {
            _pheromone[x, y] += value;
        }

        public void Set(int x, int y, int value)
        {
            _pheromone[x, y] = value;
        }

    }
}