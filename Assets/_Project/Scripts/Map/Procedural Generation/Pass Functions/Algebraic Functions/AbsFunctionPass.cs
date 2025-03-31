using UnityEngine;

[CreateAssetMenu(fileName = "AbsFunctionPass", menuName = "Core/ProcGen/Pass/Algebraic/Abs Function")]
public class AbsFunctionPass : PassDataBase
{
    public override float[,] MakePass(int dimensions, System.Random random = null, float[,] map = null)
    {
        if (map != null)
        {
            for (int i = 0; i < dimensions; i++)
            {
                for (int j = 0; j < dimensions; j++)
                {
                    map[i, j] = Mathf.Abs(map[i, j]);
                }
            }
        }
        return map;
    }
}
