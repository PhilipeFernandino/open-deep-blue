using Coimbra;
using Coimbra.Services;
using Core.Util;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Core.Map
{
    public class MapToTileBase : Actor, IMapToTibeBaleService
    {
        private TilesSettings _tilesSettings; // get

        protected override void OnInitialize()
        {
            base.OnInitialize();

            ServiceLocator.Set<IMapToTibeBaleService>(this);
        }

        protected override void OnSpawn()
        {
            base.OnSpawn();
        }

        public async UniTask<Map> GenerateTilemap(Map map)
        {
            var mapMetadata = map.Metadata;

            int dimensions = mapMetadata.Dimensions;
            TileBase[] tiles = new TileBase[dimensions * dimensions];
            TileBase[] floorTiles = new TileBase[dimensions * dimensions];


            for (int w = 0; w < dimensions; w++)
            {
                for (int h = 0; h < dimensions; h++)
                {
                    int index = w * dimensions + h;
                    tiles[index] = _tilesSettings.GetTileBase(mapMetadata.Tiles[h, w]);
                    floorTiles[index] = _tilesSettings.GetFloorTileBase(mapMetadata.BiomeTiles[h, w]);
                }
            }

            return new(mapMetadata, tiles, floorTiles);
        }
    }
}
