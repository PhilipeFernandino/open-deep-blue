using UnityEngine;

[CreateAssetMenu(fileName = "SineFunctionPass", menuName = "Pass/Algebraic/Sine Function")]
public class SineFunctionPass : PassDataBase
{
    public override float[,] MakePass(int dimensions, float[,] map = null)
    {
        if (map != null)
        {
            for (int i = 0; i < dimensions; i++)
            {
                for (int j = 0; j < dimensions; j++)
                {
                    map[i, j] = Mathf.Sin(map[i, j]);
                }
            }
        }
        return map;
    }
}
