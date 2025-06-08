using Coimbra.Services;
using Core.Level;
using Core.Util;
using System.Collections.Generic;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class ChemicalGridSensorComponent : SensorComponent
{
    [Header("Sensor Settings")]
    [Tooltip("Size of the grid to observe around the agent. Must be an odd number.")]
    public int chunkSize = 9;

    [Tooltip("The list of chemicals this sensor will observe.")]
    public List<Chemical> chemicalsToObserve = new();

    // You can optionally give it a unique name if you plan to have multiple chemical sensors on one agent
    public string sensorName = "ChemicalGridSensor";

    public override ISensor[] CreateSensors()
    {
        var chemicalService = ServiceLocator.Get<IChemicalGridService>();

        return new ISensor[]
        {
            new ChemicalGridSensor(chemicalService, transform, chunkSize, chemicalsToObserve, sensorName)
        };
    }
}