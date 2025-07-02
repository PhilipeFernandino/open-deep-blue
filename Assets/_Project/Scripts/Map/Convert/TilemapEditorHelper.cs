using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Core.Map
{
    public class TilemapEditorHelper : MonoBehaviour
    {
        [SerializeField] private Tilemap _targetTilemap;
        [SerializeField] private TilemapAsset _sourceAsset;
        [SerializeField] private TileBaseMappingSO _tileMapping;

        public Tilemap TargetTilemap => _targetTilemap;
        public TilemapAsset SourceAsset => _sourceAsset;
        public List<TileBaseMapping> TileMappings => _tileMapping.TileBaseTiles;
    }
}