using System.Collections;
using UnityEngine;

public static class Vector3IntExtensions
{
    public static Vector2Int XY(this Vector3Int vector)
    {
        return new Vector2Int(vector.x, vector.y);
    }

    public static Vector2Int XZ(this Vector3Int vector)
    {
        return new Vector2Int(vector.x, vector.z);
    }

    public static Vector2Int YZ(this Vector3Int vector)
    {
        return new Vector2Int(vector.y, vector.z);
    }

    public static (float, float, float) Spread(this Vector3Int vector)
    {
        return (vector.x, vector.y, vector.z);
    }
}
