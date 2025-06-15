using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Core.Map
{
    public class TilemapEditorHelper : MonoBehaviour
    {
        [Tooltip("The Tilemap in the scene to use as the visual editor.")]
        [SerializeField] private Tilemap _targetTilemap;

        [Tooltip("The TilemapAsset to load from and save to.")]
        [SerializeField] private TilemapAsset _sourceAsset;

        [Tooltip("A list that maps your Tile enums to their corresponding TileBase assets.")]
        [SerializeField] private TileBaseMappingSO _tileMapping;

        public Tilemap TargetTilemap => _targetTilemap;
        public TilemapAsset SourceAsset => _sourceAsset;
        public List<TileBaseMapping> TileMappings => _tileMapping.TileBaseTiles;
    }
}