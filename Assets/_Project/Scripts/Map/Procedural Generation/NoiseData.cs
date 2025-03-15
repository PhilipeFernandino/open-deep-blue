using NaughtyAttributes;
using System;
using UnityEngine;
using static FastNoiseLite;

[CreateAssetMenu(fileName = "Noise Data")]
public class NoiseData : ScriptableObject
{
    [Header("General")]

    [SerializeField] private NoiseType _noiseType;
    [SerializeField, Range(0f, 1f)] private float _frequency = 0.01f;

    [Header("Cellular Noise Parameters")]

    [SerializeField, ShowIf(nameof(CellularNoiseType))]
    private CellularDistanceFunction _cellularDistanceFunction;

    [SerializeField, ShowIf(nameof(CellularNoiseType))]
    private CellularReturnType _cellularReturnType;

    [SerializeField, Range(0f, 1f), ShowIf(nameof(CellularNoiseType))]
    private float _cellularJitter = 1f;

    [Header("Fractal Type Parameters")]

    [SerializeField] private FractalType _fractalType;
    [SerializeField, ShowIf(nameof(FractalTypeIsSet))]
    private int _fractalOctaves = 2;

    [SerializeField, Range(0f, 1f), ShowIf(nameof(FractalTypeIsSet))]
    private float _fractalWeightedStrength = 1f;

    [SerializeField, ShowIf(nameof(FractalTypeIsSet))]
    private float _fractalLacunarity = 2f;

    [SerializeField, ShowIf(nameof(FractalTypeIsSet))]
    private float _fractalGain = 0.5f;

    [SerializeField, ShowIf(nameof(FractalTypeIsPingPong))]
    private float _fractalPingPongStrength = 2f;

    #region Domain Warp
    [Header("Domain Warp")]

    [SerializeField] private bool _applyDomainWarp;

    [SerializeField, ShowIf("_applyDomainWarp")]
    private DomainWarpType _domainWarpType;

    [SerializeField, ShowIf("_applyDomainWarp")]
    private int _domainWarpAmplitude = 30;

    [SerializeField, Range(0f, 0.1f), ShowIf("_applyDomainWarp")]
    private float _domainWarpFrequency = 0.02f;

    [SerializeField, ShowIf("_applyDomainWarp")]
    private FractalType _domainWarpFractalType;

    [SerializeField, ShowIf(nameof(DomainWarpFractalType))]
    private int _domainWarpFractalOctaves = 2;

    [SerializeField, ShowIf(nameof(DomainWarpFractalType))]
    private float _domainWarpFractalLacunarity = 2f;

    [SerializeField, ShowIf(nameof(DomainWarpFractalType))]
    private float _domainWarpFractalGain = 0.5f;

    [SerializeField, ShowIf(nameof(DomainWarpFractalType))]
    private float _domainWarpFractalWeightedStrength = 1f;
    #endregion

    private FastNoiseLite _fastNoiseLite;
    private FastNoiseLite _domainWarpNoise;

    public FastNoiseLite Noise => _fastNoiseLite;

    public float GetNoise(double x, double y)
    {
        double warpedX = x;
        double warpedY = y;

        if (_applyDomainWarp)
        {
            _domainWarpNoise.DomainWarp(ref warpedX, ref warpedY);
        }

        return _fastNoiseLite.GetNoise(warpedX, warpedY);
    }

    public void Setup(int seed)
    {
        _fastNoiseLite = new(seed);

        _fastNoiseLite.SetNoiseType(_noiseType);
        _fastNoiseLite.SetFrequency(_frequency);

        if (_noiseType == NoiseType.Cellular)
        {
            _fastNoiseLite.SetCellularDistanceFunction(_cellularDistanceFunction);
            _fastNoiseLite.SetCellularJitter(_cellularJitter);
            _fastNoiseLite.SetCellularReturnType(_cellularReturnType);
        }

        if (_fractalType != FractalType.None)
        {
            _fastNoiseLite.SetFractalType(_fractalType);

            _fastNoiseLite.SetFractalOctaves(_fractalOctaves);
            _fastNoiseLite.SetFractalWeightedStrength(_fractalWeightedStrength);
            _fastNoiseLite.SetFractalLacunarity(_fractalLacunarity);
            _fastNoiseLite.SetFractalGain(_fractalGain);

            _fastNoiseLite.SetFractalPingPongStrength(_fractalPingPongStrength);
        }


        if (_applyDomainWarp)
        {
            _domainWarpNoise = new(seed + 100);

            _domainWarpNoise.SetDomainWarpType(_domainWarpType);
            _domainWarpNoise.SetDomainWarpAmp(_domainWarpAmplitude);
            _domainWarpNoise.SetFrequency(_domainWarpFrequency);

            if (_domainWarpFractalType != FractalType.None)
            {
                _domainWarpNoise.SetFractalType(_domainWarpFractalType);

                _domainWarpNoise.SetFractalWeightedStrength(_domainWarpFractalWeightedStrength);
                _domainWarpNoise.SetFractalOctaves(_domainWarpFractalOctaves);
                _domainWarpNoise.SetFractalLacunarity(_domainWarpFractalLacunarity);
                _domainWarpNoise.SetFractalGain(_domainWarpFractalGain);
            }
        }
    }

    #region Editor and Naughty Attributes Methods
    private bool CellularNoiseType() => _noiseType == FastNoiseLite.NoiseType.Cellular;

    private bool FractalTypeIsSet() => _fractalType != FastNoiseLite.FractalType.None;

    private bool FractalTypeIsPingPong() => _fractalType == FastNoiseLite.FractalType.PingPong;

    private bool DomainWarpFractalType() => _applyDomainWarp && _domainWarpFractalType != FastNoiseLite.FractalType.None;

    #endregion

}