using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;

namespace Core.Level
{
    [BurstCompile]
    public struct DecayJob : IJobParallelFor
    {
        public NativeArray<float> Grid;
        public float EvaporationMultiplier;

        public void Execute(int index)
        {
            Grid[index] *= EvaporationMultiplier;
        }
    }
}