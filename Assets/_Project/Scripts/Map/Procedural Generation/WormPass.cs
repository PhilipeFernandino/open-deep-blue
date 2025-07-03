using Core.ProcGen;
using Core.Util;
using NaughtyAttributes;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using static Core.Util.Random;
using static Helper;
using Debug = UnityEngine.Debug;

[CreateAssetMenu(fileName = "PerlinWormPass", menuName = "Core/ProcGen/Pass/Perlin Worm")]
public class WormPass : PassDataBase, IMapCreator
{
    [Header("General Parameters")]
    [SerializeField] private bool _rawResult = false;

    [Header("Worm Parameters")]
    [SerializeField] private int _worms = 1;
    [SerializeField] private int _childs = 1;
    [SerializeField] private float _childRate = 0.5f;
    [SerializeField] private float _childIterationMultiplier = 0.5f;
    [SerializeField] private float _roomOnChildChance;
    [SerializeField] private float _angleBias = 1f;

    [SerializeField] private float _speed = 3f;
    [SerializeField] private int _maxIterations = 100;

    [Header("Worm Noise Parameters")]
    [SerializeField] private Vector2 _noiseOffset = Vector2.zero;
    [SerializeField, Expandable] private NoiseData _wormNoiseData;

    [Header("Lair Parameters")]
    [SerializeField] private int _lairSizeBase = 15;
    [SerializeField] private float _lairSizeMinVariation = 0.5f;
    [SerializeField] private float _lairSizeMaxVariation = 2f;
    [SerializeField] private float _lairNoiseReachInfluence = 0.5f;
    [SerializeField] private float _lairDistanceReachInfluence = 0.5f;
    [SerializeField] private float _lairStep = 0.5f;

    [Header("Lair Noise Parameters")]
    [SerializeField, Expandable] private NoiseData _lairNoiseData;

    [Header("Reach")]
    [SerializeField, MinMaxSlider(3, 50)] private Vector2 _reachRange;
    [SerializeField, Range(0f, 1f)] private float _expandRate = 0.95f;
    [SerializeField, Range(0f, 1f)] private float _expandChildRate = 0.9f;
    [SerializeField, Range(0f, 1f)] private float _step = 0.5f;
    [SerializeField, Range(0f, 1f)] private float _distanceReachInfluence = 0.75f;
    [SerializeField, Range(0f, 1f)] private float _noiseReachInfluence = 0.25f;


    [Header("Reach Noise Parameters")]
    [SerializeField, Expandable] private NoiseData _wormReachData;

    private int _dimensions;

    private List<float> _angleValues;
    private List<float> _noiseValues;
    private List<float> _xValues;
    private List<float> _yValues;

    public List<Vector2Int> Rooms { get; private set; }
    public List<Vector2Int> CaveDeadEnds { get; private set; }

    public float[,] CreateMap(int dimensions, System.Random random)
    {
        return MakePass(dimensions, random, null);
    }

    public override float[,] MakePass(int dimensions, System.Random rng, float[,] map = null)
    {
        _angleValues = new();
        _noiseValues = new();
        _xValues = new();
        _yValues = new();

        _dimensions = dimensions;

        Rooms = new();
        CaveDeadEnds = new();

        InitNullMap(ref map, dimensions, -1f);

        int seed = rng.Next();

        _wormNoiseData.Setup(seed);
        _wormReachData.Setup(seed + 1);
        _lairNoiseData.Setup(seed + 2);

        FastNoiseLite wormNoise = _wormNoiseData.Noise;
        FastNoiseLite reachNoise = _wormReachData.Noise;
        FastNoiseLite lairNoise = _lairNoiseData.Noise;


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

            int roomSize = (int)(_lairSizeBase * Range(rng, _lairSizeMinVariation, _lairSizeMaxVariation));

            // Expand a room in the starting point
            Expand(map, x, y, roomSize, dimensions, _lairDistanceReachInfluence, _lairNoiseReachInfluence, _lairStep, lairNoise);

            Debug.Log($"{GetType()} - Starting in ({x}, {y}), params\n" +
                $"Hashes: ({h1}, {h2}, {h3}, {h4})\n" +
                $"Noise: ({n1}, {n2})");

            float reach = Range(rng, _reachRange.x, _reachRange.y);

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
                _childs,
                _childRate,
                reach,
                _expandRate,
                _expandChildRate,
                rng);

            // The starting point of a cave is also a dead end
            AddCaveDeadEnd(x, y);

            // Expand a room in the ending point
            if (Chance.EventSuccess(0.5f, rng))
            {
                int roomSize2 = (int)(_lairSizeBase * Range(rng, _lairSizeMinVariation, _lairSizeMaxVariation));

                Expand(
                    map,
                    endX,
                    endY,
                    roomSize2,
                    dimensions,
                    _lairDistanceReachInfluence,
                    _lairNoiseReachInfluence,
                    _lairStep,
                    lairNoise);

                AddRoom(endX, endY);
            }
            else
            {
                AddCaveDeadEnd(endX, endY);
            }
        }

        sw.Stop();
        Debug.Log($"{sw.ElapsedMilliseconds} elapsed miliseconds to complete perlin worm pass");
        Debug.Log($"Angle mean = {_angleValues.Sum() / _angleValues.Count}");
        Debug.Log($"Noise mean = {_noiseValues.Sum() / _noiseValues.Count}");
        Debug.Log($"X mean = {_xValues.Sum() / _xValues.Count}");
        Debug.Log($"Y mean = {_yValues.Sum() / _yValues.Count}");

        return map;
    }

    private (int x, int y) WormWalk(
        int dimensions,
        float[,] map,
        int iterations,
        float angleBias,
        FastNoiseLite wormNoise,
        FastNoiseLite reachNoise,
        FastNoiseLite lairNoise,
        int startX,
        int startY,
        int childs,
        float childRate,
        float expandSize,
        float expandRate,
        float expandChildRate,
        System.Random rng
        )
    {
        Debug.Log($"{GetType()} - starting perlin worm");

        int remainingChilds = childs;

        int x = startX;
        int y = startY;

        startX = Hash(startX);
        startY = Hash(startY);

        float iteractiveExpandSize = expandSize;

        for (int i = 0; i < iterations; i++)
        {
            if (IsWithinMapCoordinates(x, y))
            {
                map[x, y] = 1f;
            }

            // Trying to start childs
            if (Chance.EventSuccess(1f * childs / iterations, rng) && remainingChilds > 0)
            {
                Debug.Log($"Starting child at: {x}, {y}\n" +
                    $"Iterations: {(int)(_childIterationMultiplier * iterations)}\n" +
                    $"Childs: {(childs / 2) - 1}\n" +
                    $"Remaining Childs: {remainingChilds}");

                // We don't add a dead end on the starting pos here because the worm walk departs from 
                // a cave position

                (int endX, int endY) = WormWalk(
                    dimensions,
                    map,
                    (int)(_childIterationMultiplier * iterations),
                    angleBias,
                    wormNoise,
                    reachNoise,
                    lairNoise,
                    x,
                    y,
                    (int)(childs * childRate),
                    childRate,
                    iteractiveExpandSize,
                    expandRate,
                    expandChildRate,
                    rng);


                int roomSize = (int)(_lairSizeBase * Range(rng, _lairSizeMinVariation, _lairSizeMaxVariation));

                AddCaveDeadEnd(endX, endY);

                // Expand by making a room
                if (Chance.EventSuccess(_roomOnChildChance, rng))
                {
                    Expand(map, x, y, roomSize, dimensions, _lairDistanceReachInfluence, _lairNoiseReachInfluence, _lairStep, lairNoise);
                    AddRoom(x, y);
                }

                remainingChilds--;
            }
            else
            {
                // Expand hole slightly
                iteractiveExpandSize *= expandRate;
                Expand(map, x, y, (int)iteractiveExpandSize, dimensions, _distanceReachInfluence, _noiseReachInfluence, _step, reachNoise);
            }


            // Changing direction
            // We use a x and y offset because this worm could be on the same path as it's parent, without the offset
            // it would follow the same path.
            float noise = wormNoise.GetNoise(x + startX, y + startY);
            _noiseValues.Add(noise);

            float xAngle = (noise) * angleBias * Mathf.PI * 2;
            float yAngle = (noise) * angleBias * Mathf.PI * 2;

            int addX = Mathf.RoundToInt((Mathf.Sin(xAngle) + _noiseOffset.x) * _speed);
            int addY = Mathf.RoundToInt((Mathf.Cos(yAngle) + _noiseOffset.y) * _speed);
            x += addX;
            y += addY;

            _xValues.Add(Mathf.Sin(xAngle));
            _yValues.Add(Mathf.Cos(yAngle));
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
                if (IsWithinMapCoordinates(k, l))
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
                        map[k, l] = 1f;
                    }
                }
            }
        }
    }

    private void AddRoom(int x, int y)
    {
        if (IsWithinMapCoordinates(x, y))
        {
            Rooms.Add(new(x, y));
        }
    }

    private void AddCaveDeadEnd(int x, int y)
    {
        if (IsWithinMapCoordinates(x, y))
        {
            CaveDeadEnds.Add(new(x, y));
        }
    }

    private bool IsWithinMapCoordinates(int x, int y)
    {
        return IsWithinCoordinates(x, y, 0, 0, _dimensions - 1, _dimensions - 1);
    }
}
