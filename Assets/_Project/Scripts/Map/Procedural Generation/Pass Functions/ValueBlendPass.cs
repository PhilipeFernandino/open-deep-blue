using UnityEngine;

[CreateAssetMenu(fileName = "ValueBlendPass", menuName = "Core/ProcGen/Pass/Value Blend Pass")]
public class ValueBlendPass : PassDataBase
{
    [Range(-1f, 1f)]
    [SerializeField]
    private float _value;

    [SerializeField]
    private BlendMode _blendMode;

    public override float[,] MakePass(int dimensions, System.Random random = null, float[,] map = null)
    {
        return StaticMakePass(dimensions, _value, _blendMode, map);
    }

    public static float[,] StaticMakePass(int dimensions, float value, BlendMode blendMode, float[,] map = null)
    {
        for (int i = 0; i < dimensions; i++)
        {
            for (int j = 0; j < dimensions; j++)
            {
                map[i, j] = map[i, j].Blend(value, blendMode);
            }
        }

        return map;
    }
}