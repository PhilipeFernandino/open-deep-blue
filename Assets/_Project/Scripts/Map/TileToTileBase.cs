using UnityEngine;
using UnityEngine.Tilemaps;

namespace Core.Map
{
    [System.Serializable]
    public class TileToTileBase
    {
        [field: SerializeField] public Tile TileType { get; private set; }
        [field: SerializeField] public TileBase TileBase { get; private set; }
    }
}
