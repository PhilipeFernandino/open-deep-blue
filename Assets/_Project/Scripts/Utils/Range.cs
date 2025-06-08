using System.Runtime.CompilerServices;
using UnityEngine;

namespace Core.Util
{
    public static class Range
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ToClosestLowerMultiple(int value, int multiplier)
        {
            return value - value % multiplier;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ToClosestLowerMultiple(float value, int multiplier)
        {
            int vf = Mathf.RoundToInt(value);
            return vf - vf % multiplier;
        }

        /// <summary>
        /// Is within bounds [x, y)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsWithinBounds(float x, float y, float xMin, float yMin, float xMax, float yMax)
        {
            return x >= xMin && x < xMax && y >= yMin && y < yMax;
        }

        /// <summary>
        /// Is within bounds [x, y)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsWithinBounds(Vector2Int value, Vector2Int min, Vector2Int max)
        {
            return value.x >= min.x && value.x < max.x && value.y >= min.y && value.y < max.y;
        }

        /// <summary>
        /// Is within bounds [x, y)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsWithinBounds(float x, float y, float min, float max)
        {
            return IsWithinBounds(x, y, min, min, max, max);
        }

        /// <summary>
        /// Is within bounds [x, y)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsWithinBounds(Vector2Int v, float min, float max)
        {
            return IsWithinBounds(v.x, v.y, min, max);
        }
    }
}