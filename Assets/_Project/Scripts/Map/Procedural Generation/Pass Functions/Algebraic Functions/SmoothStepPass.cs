using System.Runtime.CompilerServices;
using UnityEngine;

[CreateAssetMenu(fileName = "SmoothStepFunctionPass", menuName = "Core/ProcGen/Pass/Algebraic/Smooth Step Function")]
public class SmoothStepPass : PassDataBase
{
    public override float[,] MakePass(int dimensions, System.Random random = null, float[,] map = null)
    {
        if (map != null)
        {
            for (int i = 0; i < dimensions; i++)
            {
                for (int j = 0; j < dimensions; j++)
                {
                    map[i, j] = SmoothStep(map[i, j]);
                }
            }
        }
        return map;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float SmoothStep(float value)
    {
        if (value < 0)
            value = 0;
        else if (value > 1)
            value = 1;
        else
            value = value * value * (3f - 2f * value);
        return value;
    }
}
