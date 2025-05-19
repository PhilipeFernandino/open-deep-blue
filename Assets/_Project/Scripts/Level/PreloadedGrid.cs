using Coimbra;
using Coimbra.Services;
using Coimbra.Services.Events;
using Core.EventBus;
using Core.Map;
using Core.Util;
using Cysharp.Threading.Tasks;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Core.Level
{
    public class PreloadedGrid : Actor, IGridService
    {
        [SerializeField] private Tilemap _tilemap;
        [SerializeField] private Vector2Int _startAt;

        private IMapToTileBaseService _mapToTileBase;
        private int _mapDimensions = 0;

        public int ChunkSize => _mapDimensions;

        public int LoadedDimensions => _mapDimensions;

        public int MapDimensions => _mapDimensions;


        protected override void OnInitialize()
        {
            base.OnInitialize();
            ServiceLocator.Set<IGridService>(this);

        }

        protected override void OnSpawn()
        {
            MapMetadataGeneratedEvent.AddListener(MapLoaded_EventHandler);
            _mapToTileBase = ServiceLocatorUtilities.GetServiceAssert<IMapToTileBaseService>();

        }

        private void MapLoaded_EventHandler(ref EventContext context, in MapMetadataGeneratedEvent e)
        {
            LoadMapTask(e).Forget();
        }

        private async UniTask LoadMapTask(MapMetadataGeneratedEvent e)
        {
            var mapMeta = e.MapMetadata;
            var map = await _mapToTileBase.GenerateTilemap(mapMeta);
            _mapDimensions = mapMeta.Dimensions;

            BoundsInt area = new();
            area.SetMinMax(
                Vector3Int.zero,
                new(_mapDimensions, _mapDimensions, 1));

            _tilemap.SetTilesBlock(area, map.TileBases);
        }

        public bool HasTileAt(int x, int y)
        {
            return x >= 0 && y >= 0 && _tilemap.HasTile(new Vector3Int(x, y, 0));
        }

        public bool IsTileLoaded(int x, int y)
        {
            return x >= 0 && y >= 0;
        }

        public void DamageTileAt(int x, int y, int damage)
        {
            throw new System.NotImplementedException();
        }

        public bool TryGetTileAt(int x, int y, out TileInstance tile)
        {
            throw new System.NotImplementedException();
        }

        public void LoadFloorTilesBlock(BoundsInt area, TileBase[] tiles)
        {
            throw new System.NotImplementedException();
        }

        public void LoadTilesBlock(BoundsInt area, TileBase[] tiles)
        {
            throw new System.NotImplementedException();
        }

        public bool TrySetTileAt(int x, int y, Map.Tile Tile, bool overrideTile = false)
        {
            throw new System.NotImplementedException();
        }
    }
}