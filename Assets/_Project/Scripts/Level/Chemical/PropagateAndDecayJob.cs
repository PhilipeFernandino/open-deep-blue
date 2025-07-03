using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

[BurstCompile]
public struct PropagateAndDecayJob : IJobParallelFor
{
    [ReadOnly] public NativeArray<float> ReadGrid;
    [WriteOnly] public NativeArray<float> WriteGrid;
    [ReadOnly] public NativeArray<bool>.ReadOnly ObstacleGrid;

    public int GridDimensions;
    public float DiffusionFactor;
    public float EvaporationMultiplier;
    public float PropagationDecayMultiplier;

    public void Execute(int index)
    {
        // If the current cell IS an obstacle, its scent must always be 0.
        if (ObstacleGrid[index])
        {
            WriteGrid[index] = 0f;
            return;
        }

        int x = index % GridDimensions;
        int y = index / GridDimensions;

        float centerValue = ReadGrid[index];
        float neighborSum = 0;
        int neighborCount = 0;
        float maxNeighboor = 0;

        int northIndex = index + GridDimensions;
        if (y < GridDimensions - 1 && !ObstacleGrid[northIndex])
        {
            neighborSum += ReadGrid[northIndex];
            neighborCount++;
            maxNeighboor = Mathf.Max(maxNeighboor, ReadGrid[northIndex]);
        }

        int southIndex = index - GridDimensions;
        if (y > 0 && !ObstacleGrid[southIndex])
        {
            neighborSum += ReadGrid[southIndex];
            neighborCount++;
            maxNeighboor = Mathf.Max(maxNeighboor, ReadGrid[southIndex]);
        }

        int eastIndex = index + 1;
        if (x < GridDimensions - 1 && !ObstacleGrid[eastIndex])
        {
            neighborSum += ReadGrid[eastIndex];
            neighborCount++;
            maxNeighboor = Mathf.Max(maxNeighboor, ReadGrid[eastIndex]);
        }

        int westIndex = index - 1;
        if (x > 0 && !ObstacleGrid[westIndex])
        {
            neighborSum += ReadGrid[westIndex];
            neighborCount++;
            maxNeighboor = Mathf.Max(maxNeighboor, ReadGrid[westIndex]);
        }

        float neighborAverage = neighborCount > 0 ? neighborSum / neighborCount : centerValue;

        float newValue = Mathf.Max(
            maxNeighboor * PropagationDecayMultiplier,
            centerValue * (1 - DiffusionFactor) + neighborAverage * DiffusionFactor
            );

        WriteGrid[index] = newValue * EvaporationMultiplier;
    }
}