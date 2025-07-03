using System.Runtime.CompilerServices;

namespace Core.Util
{
    public static class Random
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Range(System.Random random, float min, float max)
        {
            return (float)(random.NextDouble() * (max - min) + min);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Range(System.Random random, int min, int max)
        {
            return (int)(random.NextDouble() * (max - min) + min);
        }

    }
}