using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public static class Helper
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float TransformRange(float value, float oldMin, float oldMax, float newMin, float newMax)
    {
        return (((value - oldMin) * (newMax - newMin)) / (oldMax - oldMin)) + newMin;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float NoiseTo01Bound(float value)
    {
        return (value + 1f) / 2f;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float ToNoiseBound(float value)
    {
        return value * 2f - 1f;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsVec2IntInBoundaries(Vector2Int pos, int xMin, int yMin, int xMax, int yMax)
    {
        if (pos.x < xMin || pos.x > xMax || pos.y < yMin || pos.y > yMax)
            return false;
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsWithinCoordinates(float x, float y, float xMin, float yMin, float xMax, float yMax)
    {
        return !(x < xMin || x > xMax || y < yMin || y > yMax);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Hash(int value)
    {
        unchecked
        {
            value ^= value >> 17;
            value *= 830770091;
            value ^= value >> 11;
            value *= -1404298415;
            value ^= value >> 15;
            value *= 830770091;
            value ^= value >> 14;

            return value;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Hash(float value)
    {
        byte[] bytes = BitConverter.GetBytes(value);
        int x = BitConverter.ToInt32(bytes, 0);
        return Hash(x);
    }

    // It uses ref so we can reassign the variable 
    public static void InitMap<T>(ref T[,] map, int dimensions, T value = default) where T : IConvertible
    {
        map = new T[dimensions, dimensions];

        for (int i = 0; i < dimensions; i++)
        {
            for (int j = 0; j < dimensions; j++)
            {
                map[i, j] = value;
            }
        }
    }

    public static void InitNullMap<T>(ref T[,] map, int dimensions, T value = default) where T : IConvertible
    {
        if (map == null)
        {
            InitMap<T>(ref map, dimensions, value);
        }
    }

    // Slow. Use only to debug.
    public static Dictionary<float, int> CountValues(float[,] map, int dimensions)
    {
        Dictionary<float, int> values = new();

        for (int i = 0; i < dimensions; i++)
        {
            for (int j = 0; j < dimensions; j++)
            {
                float v = map[i, j];
                if (values.ContainsKey(v))
                {
                    values[v]++;
                }
                else
                {
                    values.Add(v, 1);
                }
            }
        }

        return values;
    }
}