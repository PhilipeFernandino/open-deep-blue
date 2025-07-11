﻿using System;
using System.Collections;
using UnityEngine;

public static class Vector2IntExtensions
{
    public static Tuple<int, int> ToTuple(this Vector2Int vector)
    {
        return new Tuple<int, int>(vector.x, vector.y);
    }

    public static Vector2Int Scalar(this Vector2Int vector, int scalar)
    {
        return new Vector2Int(vector.x * scalar, vector.y * scalar);
    }

    public static int ManhattanDistance(this Vector2Int a, Vector2Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }
}
