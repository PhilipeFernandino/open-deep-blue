using Coimbra.Services;
using Core.Level;
using Core.Util;
using System.Collections.Generic;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class ChemicalGridSensorComponent : SensorComponent
{
    [SerializeField] public int _chunkSize = 9;
    [SerializeField] public List<Chemical> _chemicalsToObserve = new();
    [SerializeField] public List<Core.Map.Tile> _tilesToObserve = new();

    // You can optionally give it a unique name if you plan to have multiple chemical sensors on one agent
    public string sensorName = "ChemicalGridSensor";

    public override ISensor[] CreateSensors()
    {
        var chemicalService = ServiceLocator.Get<IChemicalGridService>();
        var gridService = ServiceLocator.Get<IGridService>();
        var obstacleService = ServiceLocator.Get<IObstacleService>();

        return new ISensor[]
        {
            new ChemicalGridSensor(
                chemicalService,
                gridService,
                obstacleService,
                transform,
                _chunkSize,
                _chemicalsToObserve,
                _tilesToObserve,
                sensorName
            )
    };
    }
}