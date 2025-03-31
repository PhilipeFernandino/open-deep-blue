using UnityEngine;

[CreateAssetMenu(fileName = "TangentFunctionPass", menuName = "Core/ProcGen/Pass/Algebraic/Tangent Function")]
public class TangentFunctionPass : PassDataBase
{
    public override float[,] MakePass(int dimensions, System.Random random = null, float[,] values = null)
    {
        if (values != null)
        {
            for (int i = 0; i < dimensions; i++)
            {
                for (int j = 0; j < dimensions; j++)
                {
                    values[i, j] = Mathf.Tan(values[i, j]);
                }
            }
        }
        return values;
    }
}
