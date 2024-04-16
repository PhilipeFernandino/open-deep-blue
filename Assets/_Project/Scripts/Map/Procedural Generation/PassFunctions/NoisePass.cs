using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(menuName = "Pass/Noise Pass")]
public class NoisePass : PassDataBase
{
    [SerializeField]
    private BlendMode _blendMode;

    [Expandable]
    [SerializeField]
    private NoiseMapData _noiseMap;

    public override float[,] MakePass(int dimensions, float[,] map = null)
    {
        float[,] noiseValues = _noiseMap.GetNoiseMap(dimensions);

        if (map != null)
        {
            map.Blend(noiseValues, dimensions, _blendMode);
        }

        Debug.Log(map[128, 128]);

        return map;
    }
}