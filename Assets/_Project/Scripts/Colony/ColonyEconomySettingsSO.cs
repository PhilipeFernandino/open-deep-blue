using Coimbra;
using Core.Level.Dynamic;
using Core.Unit;
using NaughtyAttributes;
using UnityEngine;

namespace Core.Train
{
    [ProjectSettings("Core")]
    public class ColonyEconomySettings : ScriptableSettings
    {
        [field: SerializeField] public float FungusFeedAntsAmount { get; private set; } = 15f;
        [field: SerializeField] public float LeafFeedFungusAmount { get; private set; } = 15f;

        [field: SerializeField, Expandable] public QueenDefinition QueenDefinition { get; private set; }
        [field: SerializeField, Expandable] public FungusDefinition FungusDefinition { get; private set; }
        [field: SerializeField, Expandable] public FoodDefinition FoodDefinition { get; private set; }
        [field: SerializeField, Expandable] public AntDefinitionSO AntDefinition { get; private set; }


    }
}