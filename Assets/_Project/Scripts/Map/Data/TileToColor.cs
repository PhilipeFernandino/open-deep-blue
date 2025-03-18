using System;
using System.Linq;
using UnityEngine;

namespace Core.Map
{
    [Serializable]
    public struct TileToColor
    {
        [field: SerializeField] public Tile Tile { get; private set; }
        [field: SerializeField] public Color Color { get; private set; }
    }
}
