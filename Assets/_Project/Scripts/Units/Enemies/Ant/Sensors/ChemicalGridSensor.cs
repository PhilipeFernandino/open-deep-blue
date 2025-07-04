﻿using Core.Level;
using System.Collections.Generic;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class ChemicalGridSensor : ISensor
{
    private readonly IChemicalGridService _chemicalService;

    private readonly Transform _agentTransform;
    private readonly int _chunkSize;

    private readonly List<Chemical> _chemicalsToObserve;

    private readonly ObservationSpec _observationSpec;

    private readonly string _name;

    public ChemicalGridSensor(
        IChemicalGridService chemicalService,
        Transform agentTransform,
        int chunkSize,
        List<Chemical> chemicalsToObserve,
        string name = "ChemicalGridSensor"
    )
    {
        _chemicalService = chemicalService;
        _agentTransform = agentTransform;
        _chunkSize = chunkSize;
        _chemicalsToObserve = chemicalsToObserve;
        _name = name;

        _observationSpec = ObservationSpec.Visual(
            _chemicalsToObserve.Count,
            _chunkSize,
            _chunkSize
        );
    }

    private int GetObservationSize()
    {
        return _chunkSize * _chunkSize * _chemicalsToObserve.Count;
    }

    public ObservationSpec GetObservationSpec()
    {
        return _observationSpec;
    }

    public int Write(ObservationWriter writer)
    {
        if (_chemicalService == null)
        {
            for (int i = 0; i < GetObservationSize(); i++)
            {
                writer[i] = 0f;
            }

            Debug.LogWarning($"{GetType()} - {nameof(_chemicalService)} is null");
            return GetObservationSize();
        }

        int antX = (int)_agentTransform.position.x;
        int antY = (int)_agentTransform.position.y;
        int halfChunk = _chunkSize / 2;

        for (int c = 0; c < _chemicalsToObserve.Count; c++)
        {
            Chemical chemicalType = _chemicalsToObserve[c];
            for (int y = 0; y < _chunkSize; y++)
            {
                for (int x = 0; x < _chunkSize; x++)
                {
                    int worldX = antX + (x - halfChunk);
                    int worldY = antY + (y - halfChunk);
                    float value = _chemicalService.Get(worldX, worldY, chemicalType) / 255f;

                    writer[c, y, x] = value;
                }
            }
        }

        return GetObservationSize();
    }

    public void Update() { }
    public void Reset() { }
    public CompressionSpec GetCompressionSpec() => CompressionSpec.Default();
    public byte[] GetCompressedObservation() => null;
    public string GetName() => _name;
}