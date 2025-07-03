using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace Core.Level
{
    [BurstCompile]
    public struct SumAllJob : IJobParallelFor
    {
        public NativeArray<float> Grid;
        public float ValueToAdd;

        public void Execute(int index)
        {
            Grid[index] = Mathf.Clamp(Grid[index] + ValueToAdd, 0f, 255f);
        }
    }
}