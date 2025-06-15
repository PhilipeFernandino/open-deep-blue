using UnityEngine;
using UnityEngine.Tilemaps;

namespace Core.Map
{
    [System.Serializable]
    public class TileBaseMapping
    {
        [field: SerializeField] public Tile Tile { get; private set; }
        [field: SerializeField] public TileBase TileBase { get; private set; }
    }
}