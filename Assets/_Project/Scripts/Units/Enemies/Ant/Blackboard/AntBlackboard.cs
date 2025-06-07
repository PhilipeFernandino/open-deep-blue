using Core.ItemSystem;
using Core.Level;
using Core.Map;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Units
{
    public class AntBlackboard
    {
        private Item _carryingItem;

        public Vector2 MovingDirection;
        public float MovementSpeed;
        public float AggroDistance;

        public Item CarryingItem
        {
            get => _carryingItem;
            set
            {
                _carryingItem = value;
                CarryingItemChanged?.Invoke(_carryingItem);
            }
        }
        public TileInstance TileAhead;

        public Dictionary<AntPheromone, float[]> Pheromone;

        public event Action<Item> CarryingItemChanged;
    }
}