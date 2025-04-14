using UnityEngine;

public static class TransformExtension
{
    public static void CopyProperties(this Transform target, Transform other)
    {
        target.position = other.position;
        target.eulerAngles = other.eulerAngles;
        target.localScale = other.localScale;
    }
}
