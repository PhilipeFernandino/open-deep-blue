using System;
using System.Linq;
using UnityEngine;
using NaughtyAttributes;

namespace Core.Map
{
    [Serializable]
    public struct ValueToTile
    {
        [field: SerializeField, MinMaxSlider(-1f, 1f)] public Vector2 Range { get; private set; }
        [field: SerializeField] public Tile Tile { get; private set; }
    }
}
