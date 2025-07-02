using UnityEngine;

public static class BoundsExtensions
{
    public static Vector2 RandomPoint2D(this Bounds bounds)
    {
        return new Vector2(
            Random.Range(bounds.min.x, bounds.max.x),
            Random.Range(bounds.min.y, bounds.max.y)
        );
    }
}