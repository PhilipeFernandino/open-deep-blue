using Unity.Collections;
using UnityEngine;

namespace Core.Level
{
    public class ChemicalMap
    {
        public NativeArray<float> Grid;
        private int _dimensions;

        public ChemicalMap(int dimensions, Allocator allocator)
        {
            _dimensions = dimensions;
            Grid = new NativeArray<float>(dimensions * dimensions, allocator);
        }

        public float Get(int x, int y)
        {
            if (!Util.Range.IsWithinBounds(x, y, 0, _dimensions))
            {
                return 0f;
            }

            int index = y * _dimensions + x;
            return Grid[index];
        }

        public void Sum(int x, int y, float value)
        {
            if (!Util.Range.IsWithinBounds(x, y, 0, _dimensions))
            {
                return;
            }

            int index = y * _dimensions + x;
            Grid[index] = Mathf.Clamp(Grid[index] + value, 0f, 255f);
        }

        public void Set(int x, int y, float value)
        {
            if (!Util.Range.IsWithinBounds(x, y, 0, _dimensions))
            {
                return;
            }

            int index = y * _dimensions + x;
            Grid[index] = Mathf.Clamp(value, 0f, 255f);
        }

        public void Dispose()
        {
            if (Grid.IsCreated)
            {
                Grid.Dispose();
            }
        }
    }
}