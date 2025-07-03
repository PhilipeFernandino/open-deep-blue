using UnityEngine;

[CreateAssetMenu(menuName = "Core/ProcGen/Pass/Blank Pass")]
public class BlankPass : PassDataBase
{
    [SerializeField, Range(-1f, 1f)] private float _value;

    public override float[,] MakePass(int dimensions, System.Random random = null, float[,] map = null)
    {
        map = new float[dimensions, dimensions];

        for (int i = 0; i < dimensions; i++)
            for (int j = 0; j < dimensions; j++)
                map[i, j] = _value;

        return map;
    }
}