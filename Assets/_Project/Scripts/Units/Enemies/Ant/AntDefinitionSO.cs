using UnityEngine;

namespace Core.Unit
{
    [CreateAssetMenu(menuName = "Core/Dynamic/Ant Definition")]
    public class AntDefinitionSO : ScriptableObject
    {
        [Header("Attributes")]
        public float AttackDistance = 1;
        public float AttackDamage = 5;
        public float AggroDistance = 1;
        public float DigDamage = 5;
        public float MovementSpeed = 2;

        [Header("Saciety")]
        public float MaxSaciety = 50;
        public float SacietyLoss = 1;

        [Header("Energy")]
        public float EnergyLoss = 0.5f;
        public float MaxEnergy = 100;
        public float EnergyRegenerationThreshold = 0.3f;
        public float EnergyRegenerationRate = 1f;

        [Header("Pheromone")]
        public float DropFoodPheromone = 22.5f;
        public float DropPresencePheromone = 10f;
        public float DropThreatPheromone = 25f;

        [Header("Action")]
        public float DigEnergyCost = 10f;
        public float GatherLeafCost = 1f;
        public float CarryLeafCost = 0.2f;
    }
}