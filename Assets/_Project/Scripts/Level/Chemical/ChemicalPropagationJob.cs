using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

[BurstCompile]
public struct ChemicalPropagationJob : IJobParallelFor
{
    [ReadOnly] public NativeArray<float> ReadGrid;
    [WriteOnly] public NativeArray<float> WriteGrid;
    [ReadOnly] public NativeArray<bool>.ReadOnly ObstacleGrid;

    public int GridDimensions;
    public float DecayAmount;

    public void Execute(int index)
    {
        if (ObstacleGrid[index])
        {
            WriteGrid[index] = 0f;
            return;
        }

        float maxNeighborScent = 0f;

        int x = index % GridDimensions;
        int y = index / GridDimensions;

        int upIndex = index + GridDimensions;
        int downIndex = index - GridDimensions;
        int rightIndex = index + 1;
        int leftIndex = index - 1;

        if (y < (GridDimensions - 1) && !ObstacleGrid[upIndex])
        {
            maxNeighborScent = Mathf.Max(maxNeighborScent, ReadGrid[upIndex]);
        }
        if (y > 0 && !ObstacleGrid[downIndex])
        {
            maxNeighborScent = Mathf.Max(maxNeighborScent, ReadGrid[downIndex]);
        }
        if (x < (GridDimensions - 1) && !ObstacleGrid[rightIndex])
        {
            maxNeighborScent = Mathf.Max(maxNeighborScent, ReadGrid[rightIndex]);
        }
        if (x > 0 && !ObstacleGrid[leftIndex])
        {
            maxNeighborScent = Mathf.Max(maxNeighborScent, ReadGrid[leftIndex]);
        }

        float newScent = Mathf.Max(0, maxNeighborScent - DecayAmount);
        WriteGrid[index] = Mathf.Max(newScent, ReadGrid[index]);
    }
}