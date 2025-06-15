using System;
using UnityEngine;

namespace Core.Level
{
    [CreateAssetMenu(fileName = "ChemicalDefinition", menuName = "Core/Chemical/Chemical Definition")]
    public class ChemicalDefinition : ScriptableObject
    {
        [field: SerializeField] public Chemical ChemicalType { get; private set; }
        [field: SerializeField][field: Range(0.9f, 1f)] public float DecayRate { get; private set; }
        [field: SerializeField][field: Range(0f, 1f)] public float DiffusionFactor { get; private set; }
        [field: SerializeField][field: Range(0f, 1f)] public float PropagationFactor { get; private set; }

    }
}