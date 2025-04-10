using Coimbra.Services;
using Core.Map;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Core.Level
{
    [DynamicService]
    public interface IGridService : IService
    {
        public int ChunkSize { get; }
        public int LoadedDimensions { get; }
        public int MapDimensions { get; }

        public void DrawInGrid(Vector2 position, in Vector2Int size) { }
        public bool HasTileAt(int x, int y);
        public void LoadTilesBlock(BoundsInt area, TileBase[] tiles);
        public void LoadFloorTilesBlock(BoundsInt area, TileBase[] tiles);
        public void SetTileAt(int x, int y, Map.Tile tile);
        public bool TryGetTileAt(int x, int y, out TileInstance tile);
        public void DamageTileAt(int x, int y, ushort damage);
        public bool IsTileLoaded(int x, int y);



        public bool HasTileAt(Vector2 v)
        {
            var pos = ToGridPosition(v);
            return HasTileAt(pos.x, pos.y);
        }


        public void SetTileAt(Vector2 v, Map.Tile tile)
        {
            var pos = ToGridPosition(v);
            SetTileAt(pos.x, pos.y, tile);
        }

        public bool TryGetTileAt(Vector2 v, out TileInstance tile)
        {
            var pos = ToGridPosition(v);
            return TryGetTileAt(pos.x, pos.y, out tile);
        }

        public void DamageTileAt(Vector2 v, ushort damage)
        {
            var pos = ToGridPosition(v);
            DamageTileAt(pos.x, pos.y, damage);
        }

        public bool IsTileLoaded(Vector2 v)
        {
            var pos = ToGridPosition(v);
            return IsTileLoaded(pos.x, pos.y);
        }

        public static (int x, int y) ToGridPosition(Vector2 v)
        {
            Vector2Int v2 = new(Mathf.RoundToInt(v.x), Mathf.RoundToInt(v.y));
            return (v2.x, v2.y);
        }
    }
}