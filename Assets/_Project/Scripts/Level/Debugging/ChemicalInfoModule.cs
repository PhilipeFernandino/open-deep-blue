﻿using Core.Level;
using Core.Map;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Core.Debugger
{
    public struct ChemicalStrength
    {
        public Chemical ChemicalType;
        public float Strength;
    }

    public struct ChemicalDebugData
    {
        public List<ChemicalStrength> Chemicals;
    }

    [CreateAssetMenu(fileName = "Chemical Info Module", menuName = "Core/Debugger/Modules/Chemical Info Module")]
    public class ChemicalInfoModule : DebugModuleSO
    {
        private readonly StringBuilder _stringBuilder = new StringBuilder();

        public override void UpdateData(object data)
        {
            if (data is ChemicalDebugData chemicalData)
            {
                _stringBuilder.Clear();

                foreach (var chemical in chemicalData.Chemicals)
                {
                    _stringBuilder.AppendLine($"{chemical.ChemicalType}: {chemical.Strength:0.##}");
                }

                DisplayText = _stringBuilder.ToString();
            }
        }

        public override void ResetData()
        {
            DisplayText = "Out of bounds.";
        }
    }
}