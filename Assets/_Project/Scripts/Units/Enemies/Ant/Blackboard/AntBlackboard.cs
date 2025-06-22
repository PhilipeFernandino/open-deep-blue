using Core.ItemSystem;
using System;
using UnityEngine;

namespace Core.Units
{
    public class AntBlackboard : MonoBehaviour
    {
        [Header("Saciety")]
        public float SacietyLoss = 1;
        public float MaxSaciety = 50;

        [Header("Combat")]
        public float AttackDistance = 1;
        public float AttackDamage = 5;
        public float AggroDistance = 1;

        [Header("General")]
        public float DigDamage = 5;
        public float MovementSpeed = 2;

        [HideInInspector] public float Health;
        [HideInInspector] public float CumulativeReward;
        [HideInInspector] public Vector2 MovingDirection;

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
    }
}