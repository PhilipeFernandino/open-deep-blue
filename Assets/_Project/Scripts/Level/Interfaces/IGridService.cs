using Coimbra.Services;
using Core.Map;
using System;
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

        public void ClearDrawAt(Vector2 position) { throw new NotImplementedException(); }
        public void DrawInGrid(Vector2 position, Color color) { throw new NotImplementedException(); }
        public void DrawInGrid(Vector2 position, in Vector2Int size, Color color) { throw new NotImplementedException(); }

        public bool HasTileAt(int x, int y);
        public void LoadTilesBlock(BoundsInt area, TileBase[] tiles);
        public void LoadFloorTilesBlock(BoundsInt area, TileBase[] tiles);
        public void LoadGridBlock(int dimensions, Map.Tile[,] tiles);
        public bool TrySetTileAt(int x, int y, Map.Tile tile, bool overrideTile = false);
        public bool TryGetTileAt(int x, int y, out TileInstance tile);
        public void DamageTileAt(int x, int y, int damage);
        public bool IsTileLoaded(int x, int y);



        public bool HasTileAt(Vector2 v)
        {
            var pos = ToGridPosition(v);
            return HasTileAt(pos.x, pos.y);
        }


        public bool TrySetTileAt(Vector2 v, Map.Tile tile, bool overrideTile = false)
        {
            var pos = ToGridPosition(v);
            return TrySetTileAt(pos.x, pos.y, tile, overrideTile);
        }

        public bool TryGetTileAt(Vector2 v, out TileInstance tile)
        {
            var pos = ToGridPosition(v);
            return TryGetTileAt(pos.x, pos.y, out tile);
        }

        public void DamageTileAt(Vector2 v, int damage)
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