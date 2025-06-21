using Core.Level;
using Core.Map;
using System.Collections.Generic;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class TileGridSensor : ISensor
{
    private readonly IGridService _gridService;
    private readonly IObstacleService _obstacleService;

    private readonly Transform _agentTransform;
    private readonly int _chunkSize;

    private readonly List<Tile> _tilesToObserve;

    private readonly ObservationSpec _observationSpec;

    private readonly string _name;

    public TileGridSensor(
        IGridService gridService,
        IObstacleService obstacleService,
        Transform agentTransform,
        int chunkSize,
        List<Tile> tilesToObserve,
        string name = "TileGridSensor"
    )
    {
        _gridService = gridService;
        _obstacleService = obstacleService;
        _agentTransform = agentTransform;
        _chunkSize = chunkSize;
        _tilesToObserve = tilesToObserve;
        _name = name;

        _observationSpec = ObservationSpec.Visual(
            _chunkSize,
            _chunkSize,
            _tilesToObserve.Count + 1
        );
    }

    private int GetObservationSize()
    {
        return _chunkSize * _chunkSize * (_tilesToObserve.Count + 1);
    }

    public ObservationSpec GetObservationSpec()
    {
        return _observationSpec;
    }

    public int Write(ObservationWriter writer)
    {
        if (_gridService == null)
        {
            for (int i = 0; i < GetObservationSize(); i++)
            {
                writer[i] = 0f;
            }

            Debug.LogWarning($"{GetType()} - {nameof(_gridService)} is null");
            return GetObservationSize();
        }

        int antX = (int)_agentTransform.position.x;
        int antY = (int)_agentTransform.position.y;
        int halfChunk = _chunkSize / 2;

        for (int t = 0; t < _tilesToObserve.Count; t++)
        {
            for (int y = 0; y < _chunkSize; y++)
            {
                for (int x = 0; x < _chunkSize; x++)
                {
                    int worldX = antX + (x - halfChunk);
                    int worldY = antY + (y - halfChunk);
                    var tileType = _tilesToObserve[t];

                    var tile = _gridService.Get(worldX, worldY);
                    float value = (tile.TileType == tileType) ? 1.0f : 0.0f;
                    writer[y, x, t] = value;
                }
            }
        }

        int obstacleChannel = _tilesToObserve.Count;

        for (int y = 0; y < _chunkSize; y++)
        {
            for (int x = 0; x < _chunkSize; x++)
            {
                int worldX = antX + (x - halfChunk);
                int worldY = antY + (y - halfChunk);

                float value = _obstacleService.HasObstacle(worldX, worldY) ? 1.0f : 0.0f;
                writer[y, x, obstacleChannel] = value;

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