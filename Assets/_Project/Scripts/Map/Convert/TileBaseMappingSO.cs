using System.Collections.Generic;
using UnityEngine;

namespace Core.Map
{
    [CreateAssetMenu(fileName = "Tile Base Mapping", menuName = "Core/Map/Tile Base Mapping")]

    public class TileBaseMappingSO : ScriptableObject
    {
        [field: SerializeField] public List<TileBaseMapping> TileBaseTiles { get; private set; } = new();
    }
}