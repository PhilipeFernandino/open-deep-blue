using Core.ItemSystem;
using Core.Unit;
using System;
using UnityEngine;

namespace Core.Units
{
    public class AntBlackboard : MonoBehaviour
    {
        [SerializeField] private AntDefinitionSO _antDefinition;

        // runtime variables
        [HideInInspector] public float Health;
        [HideInInspector] public float CumulativeReward;
        [HideInInspector] public Vector2 MovingDirection;

        public float MaxSaciety => _antDefinition.MaxSaciety;
        public float AttackDistance => _antDefinition.AttackDistance;
        public float AttackDamage => _antDefinition.AttackDamage;
        public float AggroDistance => _antDefinition.AggroDistance;
        public float DigDamage => _antDefinition.DigDamage;
        public float MovementSpeed => _antDefinition.MovementSpeed;
        public float SacietyLoss => _antDefinition.SacietyLoss;
        public float MaxEnergy => _antDefinition.MaxEnergy;
        public float EnergyLoss => _antDefinition.EnergyLoss;
        public float DropFoodPheromone => _antDefinition.DropFoodPheromone;
        public float DropPresencePheromone => _antDefinition.DropPresencePheromone;
        public float DropThreatPheromone => _antDefinition.DropThreatPheromone;
        public float DigEnergyCost => _antDefinition.DigEnergyCost;
        public float GatherLeafCost => _antDefinition.GatherLeafCost;
        public float CarryLeafCost => _antDefinition.CarryLeafCost;
        public float EnergyRegenerationThreshold => _antDefinition.EnergyRegenerationThreshold;
        public float EnergyRegenerationRate => _antDefinition.EnergyRegenerationRate;

        private float _energy;
        private float _saciety;
        private Item _carryingItem;

        public float SacietyPercentage => Saciety / MaxSaciety;
        public float EnergyPercentage => Energy / MaxEnergy;

        public float Saciety
        {
            set
            {
                _saciety = Mathf.Clamp(value, 0, MaxSaciety);
            }

            get => _saciety;
        }

        public float Energy
        {
            set
            {
                _energy = Mathf.Clamp(value, 0, MaxEnergy);
            }

            get => _energy;
        }

        public Item CarryingItem
        {
            get => _carryingItem;
            set
            {
                _carryingItem = value;
                CarryingItemChanged?.Invoke(_carryingItem);
            }
        }
        public bool HasEnergy(float value) => Energy >= value;

        public event Action<Item> CarryingItemChanged;
        public event Action EnergyZeroed;
        public event Action SacietyZeroed;

        public bool IsCarrying(Item item) => CarryingItem == item;
        public void GiveItem(Item item) => CarryingItem = item;

        private void Awake()
        {
            Saciety = _antDefinition.MaxSaciety;
            Energy = _antDefinition.MaxEnergy;
            _carryingItem = Item.None;
        }

    }
}