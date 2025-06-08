using Core.Map;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Level
{
    [CreateAssetMenu(fileName = "TileChemicalEmission", menuName = "Core/Chemical/Tile Emission")]
    public class TileChemicalEmission : ScriptableObject
    {
        [field: SerializeField] public Tile Tile { get; private set; }

        [Header("Chemical Properties")]
        [field: SerializeField] public List<ChemicalEmission> ChemicalEmissions { get; private set; } = new();
    }
}