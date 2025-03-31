using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(menuName = "Core/ProcGen/Pass/Noise Pass")]
public class NoisePass : PassDataBase
{
    [SerializeField]
    private BlendMode _blendMode;

    [Expandable]
    [SerializeField]
    private NoiseMapData _noiseMap;

    public override float[,] MakePass(int dimensions, System.Random rng, float[,] map = null)
    {
        float[,] noiseValues = _noiseMap.GetNoiseMap(dimensions, rng);

        if (map != null)
        {
            map.Blend(noiseValues, dimensions, _blendMode);
        }

        return map;
    }
}