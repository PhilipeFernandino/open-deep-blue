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

        private float _saciety;
        private Item _carryingItem;

        public float SacietyPercentage => Saciety / MaxSaciety;

        public float Saciety
        {
            set
            {
                _saciety = Mathf.Clamp(value, 0, MaxSaciety);
            }

            get => _saciety;
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

        public event Action<Item> CarryingItemChanged;


        public bool IsCarrying(Item item) => CarryingItem == item;
        public void GiveItem(Item item) => CarryingItem = item;
    }
}