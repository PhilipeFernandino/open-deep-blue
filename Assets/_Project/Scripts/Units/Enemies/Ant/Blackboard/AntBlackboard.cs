using Core.ItemSystem;
using Core.Level;
using Core.Map;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Units
{
    public class AntBlackboard
    {
        public Vector2 MovingDirection;
        public float MovementSpeed;
        public float AggroDistance;

        public Item CarryingItem;
        public TileInstance TileAhead;

        public Dictionary<AntPheromone, float[]> Pheromone;
    }
}