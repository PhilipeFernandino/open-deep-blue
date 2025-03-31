using System;
using System.Collections.Generic;
using Systems.GridSystem;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Core.Map
{
    [CreateAssetMenu(menuName = "Core/Map/Tile")]
    public class Tiles_SO : ScriptableObject
    {
        [field: SerializeField] public List<EnumTileData> Tiles { get; private set; }
    }

    [Serializable]
    public class EnumTileData
    {
        [field: SerializeField] public GroundTile GroundTile { get; private set; }
        [field: SerializeField] public Tile Tile { get; private set; }
    }
}