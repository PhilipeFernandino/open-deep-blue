using Coimbra.Services;
using Core.Level;
using Core.Map;
using Core.Util;
using System.Collections.Generic;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class TileGridSensorComponent : SensorComponent
{
    [SerializeField] public int _chunkSize = 9;
    [SerializeField] public List<Tile> _tilesToObserve = new();

    public string sensorName = "TileGridSensor";

    public override ISensor[] CreateSensors()
    {
        var gridService = ServiceLocator.Get<IGridService>();
        var obstacleService = ServiceLocator.Get<IObstacleService>();

        return new ISensor[]
        {
            new TileGridSensor(
                gridService,
                obstacleService,
                transform,
                _chunkSize,
                _tilesToObserve,
                sensorName
            )
        };
    }
}