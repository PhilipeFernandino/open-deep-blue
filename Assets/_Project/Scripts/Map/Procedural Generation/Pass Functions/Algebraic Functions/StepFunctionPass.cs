using UnityEngine;

[CreateAssetMenu(fileName = "StepFunctionPass", menuName = "Pass/Algebraic/Step Function")]
public class StepFunctionPass : PassDataBase
{
    [Range(-1f, 1f)]
    [SerializeField]
    private float _stepValue;

    public override float[,] MakePass(int dimensions, System.Random random = null, float[,] map = null)
    {
        if (map != null)
        {
            for (int i = 0; i < dimensions; i++)
            {
                for (int j = 0; j < dimensions; j++)
                {
                    map[i, j] = map[i, j] < _stepValue ? 0 : 1;
                }
            }
        }
        return map;
    }
}
