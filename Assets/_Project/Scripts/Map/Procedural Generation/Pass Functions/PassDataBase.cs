using UnityEngine;

public abstract class PassDataBase : ScriptableObject
{
    public abstract float[,] MakePass(int dimensions, System.Random random = null, float[,] map = null);
}