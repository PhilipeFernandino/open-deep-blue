using UnityEngine;

public static class Vector2Extension
{
    /// <returns>
    /// <see cref="Vector3"/> (<paramref name="vector"/>.x, <paramref name="thirdValue"/>, <paramref name="vector"/>.y)
    /// </returns>
    public static Vector3 XZ(this Vector2 vector, float thirdValue = 0f)
    {
        return new Vector3(vector.x, thirdValue, vector.y);
    }

    /// <returns>
    /// <see cref="Vector3"/> (<paramref name="vector"/>.x, <paramref name="vector"/>.y, <paramref name="thirdValue"/>)
    /// </returns>
    public static Vector3 XY(this Vector2 vector, float thirdValue = 0f)
    {
        return new Vector3(vector.x, vector.y, thirdValue);
    }

    /// <returns>
    /// <see cref="Vector3"/> (<paramref name="thirdValue"/>, <paramref name="vector"/>.x, <paramref name="vector"/>.y)
    /// </returns>
    public static Vector3 YZ(this Vector2 vector, float thirdValue = 0f)
    {
        return new Vector3(thirdValue, vector.x, vector.y);
    }

    /// <returns>
    /// <see cref="Vector2"/> (<paramref name="vector"/>.y, <paramref name="vector"/>.x)
    /// </returns>
    public static Vector2 YX(this Vector2 vector)
    {
        return new Vector2(vector.y, vector.x);
    }

    public static Vector2 NormalizeExcess(this Vector2 vector) => vector.sqrMagnitude > 1 ? vector.normalized : vector;

    public static float Distance(this Vector2 a, Vector2 b)
    {
        return Vector2.Distance(a, b);
    }

    public static int ManhattanDistance(this Vector2 a, Vector2 b)
    {
        var ai = Vector2Int.RoundToInt(a);
        var bi = Vector2Int.RoundToInt(b);

        return Mathf.Abs(ai.x - bi.x) + Mathf.Abs(ai.y - bi.y);
    }

    public static float OctileDistance(this Vector2 a, Vector2 b)
    {
        var ai = Vector2Int.RoundToInt(a);
        var bi = Vector2Int.RoundToInt(b);
        int dx = Mathf.Abs(ai.x - bi.x);
        int dy = Mathf.Abs(ai.y - bi.y);
        int minD = Mathf.Min(dx, dy);
        int maxD = Mathf.Max(dx, dy);
        return maxD + (minD * 0.4142f);
    }

    public static (int x, int y) DiscreteTuple(this Vector2 v)
    {
        Vector2Int v2 = new(Mathf.RoundToInt(v.x), Mathf.RoundToInt(v.y));
        return (v2.x, v2.y);
    }

    public static Vector2Int Discrete(this Vector2 v)
    {
        Vector2Int v2 = new(Mathf.RoundToInt(v.x), Mathf.RoundToInt(v.y));
        return v2;
    }
}