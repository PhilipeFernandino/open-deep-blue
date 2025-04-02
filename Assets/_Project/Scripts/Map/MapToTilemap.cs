using Coimbra;
using Coimbra.Services;
using Core.Util;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Core.Map
{
    public class MapToTilemap : Actor, ITilemapService
    {
        [SerializeField] private Tilemap _tilemap;
        [SerializeField] private Tilemap _floorTilemap;

        private IMapLevelGeneratorService _mapLevelGeneratorService;
        private TilesSettings _tilesSettings;

        private BoundsInt area;

        protected override void OnInitialize()
        {
            base.OnInitialize();

            ServiceLocator.Set<ITilemapService>(this);
        }

        protected override void OnSpawn()
        {
            base.OnSpawn();

            _mapLevelGeneratorService = ServiceLocatorUtilities.GetServiceAssert<IMapLevelGeneratorService>();
            _tilesSettings = ScriptableSettings.GetOrFind<TilesSettings>();
        }

        public async UniTask<Map> GenerateTilemap()
        {
            var mapMetadata = await _mapLevelGeneratorService.GenerateMapLevel();

            int dimensions = mapMetadata.Dimensions;
            TileBase[] tiles = new TileBase[dimensions * dimensions];
            TileBase[] floorTiles = new TileBase[dimensions * dimensions];

            area.size = new Vector3Int(dimensions, dimensions, 1);

            for (int w = 0; w < dimensions; w++)
            {
                for (int h = 0; h < dimensions; h++)
                {
                    int index = w * dimensions + h;
                    tiles[index] = _tilesSettings.GetTileBase(mapMetadata.Tiles[h, w]);
                    floorTiles[index] = _tilesSettings.GetFloorTileBase(mapMetadata.BiomeTiles[h, w]);
                }
            }

            _tilemap.SetTilesBlock(area, tiles);
            _floorTilemap.SetTilesBlock(area, floorTiles);

            return new(mapMetadata, _tilemap);
        }
    }
}
