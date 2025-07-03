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

    public string sensorName = "ChemicalGridSensor";

    public override ISensor[] CreateSensors()
    {
        var chemicalService = ServiceLocator.Get<IChemicalGridService>();
        var gridService = ServiceLocator.Get<IGridService>();

        return new ISensor[]
        {
            new ChemicalGridSensor(
                chemicalService,
                transform,
                _chunkSize,
                _chemicalsToObserve,
                sensorName
            )
        };
    }
}