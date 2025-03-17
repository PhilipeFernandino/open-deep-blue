using Core.Util;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Core.Map
{
    [System.Serializable]
    public class TileToTileBase
    {
        [field: SerializeField] public Tile Tile { get; private set; }
        [field: SerializeField] public TileBase TileBase { get; private set; }
    }


    public class MapToTilemap : MonoBehaviour
    {
        [SerializeField] private Tilemap _tilemap;
        [SerializeField] List<TileToTileBase> _tileMapping;

        public IMapLevelGeneratorService _mapLevelGeneratorService;
        private BoundsInt area;

        private void Start()
        {
            _mapLevelGeneratorService = ServiceLocatorUtilities.GetServiceAssert<IMapLevelGeneratorService>();
            ToTilemap();
        }

        private async void ToTilemap()
        {
            var map = await _mapLevelGeneratorService.GenerateMapLevel();

            int ctl = _tileMapping.Count;
            int dimensions = map.Dimensions;
            TileBase[] tiles = new TileBase[dimensions * dimensions];

            area.size = new Vector3Int(dimensions, dimensions, 1);

            for (int w = 0; w < dimensions; w++)
            {
                for (int h = 0; h < dimensions; h++)
                {
                    for (int c = 0; c < ctl; c++)
                    {
                        if (_tileMapping[c].Tile == map.Tiles[w, h])
                        {
                            tiles[w * dimensions + h] = _tileMapping[c].TileBase;
                        }
                    }
                }
            }

            _tilemap.SetTilesBlock(area, tiles);
        }
    }
}
