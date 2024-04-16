using NaughtyAttributes;
using System.Diagnostics;
using UnityEngine;
using static Helper;
using Debug = UnityEngine.Debug;

[CreateAssetMenu(fileName = "PerlinWormPass", menuName = "Pass/Perlin Worm")]
public class WormPass : PassDataBase
{
    [Header("General Parameters")]

    [SerializeField] private bool _randomizeSeed = true;
    [SerializeField] private bool _rawResult = false;
    [SerializeField] private int _seed = 0;

    [Header("Worm Parameters")]
    [SerializeField] private int _worms = 1;
    [SerializeField] private int _childs = 1;
    [SerializeField] private float _childIterationMultiplier = 0.5f;

    [SerializeField] private float _speed = 3f;
    [SerializeField] private int _maxIterations = 100;

    [Header("Lair Parameters")]
    [SerializeField] private int _lairSizeBase = 15;
    [SerializeField] private float _lairSizeMinVariation = 0.5f;
    [SerializeField] private float _lairSizeMaxVariation = 2f;
    [SerializeField] private float _lairNoiseReachInfluence = 0.5f;
    [SerializeField] private float _lairDistanceReachInfluence = 0.5f;
    [SerializeField] private float _lairStep = 0.5f;

    [Header("Lair Noise Parameters")]
    [SerializeField, Expandable] private NoiseData _lairNoiseData;

    [Header("Lair Noise Parameters")]
    [SerializeField, Expandable] private NoiseData _wormNoiseData;
    [SerializeField] private float _angleBias = 1f;

    [Header("Reach")]
    [SerializeField] private int _reach;
    [SerializeField, Range(0f, 1f)] private float _step = 0.5f;
    [SerializeField, Range(0f, 1f)] private float _distanceReachInfluence = 0.75f;
    [SerializeField, Range(0f, 1f)] private float _noiseReachInfluence = 0.25f;

    [Header("Reach Noise Parameters")]
    [SerializeField, Expandable] private NoiseData _wormReachData;

    private int RoomSize => (int)(_lairSizeBase * Random.Range(_lairSizeMinVariation, _lairSizeMaxVariation));

    public override float[,] MakePass(int dimensions, float[,] map = null)
    {
        Random.InitState(_seed);

        _wormNoiseData.Setup();
        _wormReachData.Setup();
        _lairNoiseData.Setup();

        FastNoiseLite wormNoise = _wormNoiseData.Noise;
        FastNoiseLite reachNoise = _wormReachData.Noise;
        FastNoiseLite lairNoise = _lairNoiseData.Noise;

        int seed = _seed;

        Stopwatch sw = Stopwatch.StartNew();

        for (int w = 0; w < _worms; w++)
        {
            wormNoise.SetSeed(seed);
            reachNoise.SetSeed(seed);
            lairNoise.SetSeed(seed);

            seed = Hash(seed);

            // Defining the starting point
            int h1 = Hash(seed);
            int h2 = Hash(h1);
            int h3 = Hash(h2);
            int h4 = Hash(h3);

            float n1 = NoiseTo01Bound(wormNoise.GetNoise(h1, h2));
            float n2 = NoiseTo01Bound(wormNoise.GetNoise(h3, h4));

            int x = (int)(n1 * (dimensions));
            int y = (int)(n2 * (dimensions));

            // Expand a room in the starting point
            Expand(map, x, y, RoomSize, dimensions, _lairDistanceReachInfluence, _lairNoiseReachInfluence, _lairStep, lairNoise);

            Debug.Log($"{GetType()} - Starting in ({x}, {y}), params\n" +
                $"Hashes: ({h1}, {h2}, {h3}, {h4})\n" +
                $"Noise: ({n1}, {n2})");

            (int endX, int endY) = WormWalk(
                dimensions,
                map,
                _maxIterations,
                _angleBias,
                wormNoise,
                reachNoise,
                lairNoise,
                x,
                y,
                _childs);


            // Expand a room in the ending point
            Expand(map, endX, endY, RoomSize, dimensions, _lairDistanceReachInfluence, _lairNoiseReachInfluence, _lairStep, lairNoise);

        }

        map[dimensions - 1, dimensions - 1] = 1f;

        sw.Stop();
        Debug.Log($"{sw.ElapsedMilliseconds} elapsed miliseconds to complete perlin worm pass");

        if (_randomizeSeed)
        {
            _seed = Random.Range(int.MinValue, int.MaxValue);
        }

        return map;
    }

    private (int x, int y) WormWalk(int dimensions, float[,] map, int iterations, float angleBias,
        FastNoiseLite wormNoise, FastNoiseLite reachNoise, FastNoiseLite lairNoise, int startX, int startY, int childs)
    {
        Debug.Log($"{GetType()} - starting perlin worm");

        int remainingChilds = childs;

        int x = startX;
        int y = startY;
        int xOffset = startX;
        int yOffset = startY;

        for (int i = 0; i < iterations; i++)
        {
            if (IsWithinCoordinates(x, y, 0, 0, dimensions - 1, dimensions - 1))
                map[x, y] = 1f;

            // Trying to start childs
            if (ChanceUtil.EventSuccess(1f * childs / iterations) && remainingChilds > 0)
            {
                Debug.Log($"Starting child at: {x}, {y}\n" +
                    $"Iterations: {(int)(_childIterationMultiplier * iterations)}\n" +
                    $"Childs: {(childs / 2) - 1}\n" +
                    $"Remaining Childs: {remainingChilds}");

                WormWalk(
                    dimensions,
                    map,
                    (int)(_childIterationMultiplier * iterations),
                    -angleBias,
                    wormNoise,
                    reachNoise,
                    lairNoise,
                    x,
                    y,
                    (childs / 2));

                int roomSize = (int)(_lairSizeBase * Random.Range(_lairSizeMinVariation, _lairSizeMaxVariation));

                // Expand making a room
                Expand(map, x, y, roomSize, dimensions, _lairDistanceReachInfluence, _lairNoiseReachInfluence, _lairStep, lairNoise);

                remainingChilds--;
            }
            else
            {
                // Expand hole slightly
                Expand(map, x, y, _reach, dimensions, _distanceReachInfluence, _noiseReachInfluence, _step, reachNoise);
            }


            // Changing direction
            float noise = wormNoise.GetNoise(x + xOffset, y + yOffset);

            float angle = noise * Mathf.PI * 2f * angleBias;

            x += (int)(Mathf.Sin(angle) * _speed);
            y += (int)(Mathf.Cos(angle) * _speed);
        }

        return (x, y);
    }

    private void Expand(float[,] map, int x, int y, int roomSize, int dimensions, float distanceReachInfluence, float noiseReachInfluence, float step, FastNoiseLite noise)
    {
        float maxDistance = Mathf.Sqrt(roomSize * roomSize + roomSize * roomSize);

        for (int k = x - roomSize; k < x + roomSize; k++)
        {
            for (int l = y - roomSize; l < y + roomSize; l++)
            {
                if (IsWithinCoordinates(k, l, 0, 0, dimensions - 1, dimensions - 1))
                {
                    float boundedNoise = noise.GetNoise(k, l);

                    // Distancia euclidiana relativa em relação ao centro de cada iteração. Vai de 0 a 1.
                    float a2 = Mathf.Abs(k - x) * Mathf.Abs(k - x);
                    float b2 = Mathf.Abs(l - y) * Mathf.Abs(l - y);
                    float distanceFromCenter = 1 - (Mathf.Sqrt(a2 + b2) / maxDistance);

                    // Cálculo do valor final considerando a influência da distância e do noise.
                    float boundedNoiseByDistance = distanceFromCenter * distanceReachInfluence
                        + boundedNoise * noiseReachInfluence;

                    if (_rawResult && map[k, l] < boundedNoiseByDistance)
                    {
                        map[k, l] = boundedNoiseByDistance;
                    }
                    else if (map[k, l] < boundedNoiseByDistance && boundedNoiseByDistance > step)
                    {
                        map[k, l] = 1;
                    }
                }
            }
        }
    }
}
