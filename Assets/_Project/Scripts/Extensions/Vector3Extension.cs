using UnityEngine;

public static class Vector3Extension
{
    public static Vector2 XY(this Vector3 vector)
    {
        return new Vector2(vector.x, vector.y);
    }

    public static Vector2 XZ(this Vector3 vector)
    {
        return new Vector2(vector.x, vector.z);
    }

    public static Vector2 YZ(this Vector3 vector)
    {
        return new Vector2(vector.y, vector.z);
    }

    public static (float, float, float) Spread(this Vector3 vector)
    {
        return (vector.x, vector.y, vector.z);
    }

    public static Vector3 FromValue(float value)
    {
        return new Vector3(value, value, value);
    }

    public static Vector3 GetPositionOnRange(this Vector3 origin, Vector2 input, float range)
    {
        return origin + new Vector3(input.x, 0, input.y) * range;
    }
}