using UnityEngine;

public abstract class PassDataBase : ScriptableObject
{
    public abstract float[,] MakePass(int dimensions, float[,] map = null);
}