using UnityEngine;

[CreateAssetMenu(fileName = "PowFunctionPass", menuName = "Pass/Algebraic/Pow Function")]
public class PowFunctionPass : PassDataBase
{
    [SerializeField] private float expoent;

    public override float[,] MakePass(int dimensions, System.Random random = null, float[,] map = null)
    {
        if (map != null)
        {
            for (int i = 0; i < dimensions; i++)
            {
                for (int j = 0; j < dimensions; j++)
                {
                    map[i, j] = Mathf.Pow(map[i, j], expoent);
                }
            }
        }
        return map;
    }
}
