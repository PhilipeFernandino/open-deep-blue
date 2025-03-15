using Core.ProcGen;
using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "NoiseMap")]
public class NoiseMapData : ScriptableObject, IMapCreator
{
    [Header("General")]

    [SerializeField, Expandable] private NoiseData _noise;
    [SerializeField] private bool _invert;

    [Header("Colorization")]

    [SerializeField] private Colorization _colorization = Colorization.FlatColor;

    [SerializeField, Range(-1f, 1f), HideIf(nameof(GradientColorization))]
    private float _stepValue = 0f;

    [ShowIf(nameof(GradientColorization)), SerializeField] private Gradient _colorGradient = new Gradient();

    public float[,] CreateMap(int dimensions, System.Random rng)
    {
        return GetNoiseMap(dimensions, rng);
    }

    public float[,] GetNoiseMap(int dimensions, System.Random rng)
    {
        _noise.Setup(rng.Next());

        int width = dimensions;
        int height = dimensions;

        float[,] map = new float[dimensions, dimensions];

        // Create noise map
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float noiseValue = _noise.GetNoise(x, y);

                if (_invert)
                {
                    noiseValue = -noiseValue;
                }

                if (_colorization == Colorization.FlatColor)
                {
                    map[x, y] = noiseValue < _stepValue ? 0 : 1;
                }
                else if (_colorization == Colorization.Gradient)
                {
                    map[x, y] = Helper.TransformRange(
                        _colorGradient.Evaluate(Helper.NoiseTo01Bound(noiseValue)).r,
                        0,
                        1,
                        -1,
                        1);
                }
                else
                {
                    map[x, y] = noiseValue;
                }
            }
        }

        return map;
    }

    private bool GradientColorization() => _colorization == Colorization.Gradient;
}