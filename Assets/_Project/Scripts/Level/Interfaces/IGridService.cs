using Coimbra.Services;
using Core.Map;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Core.Level
{
    [DynamicService]
    public interface IGridService : IService
    {
        public void SetTileAt(int x, int y, TileBase tileBase);
        public bool TryGetTileAt(int x, int y, out TileInstance tile);
        public void DamageTileAt(int x, int y, ushort damage);

        public void SetTileAt(Vector2 v, TileBase tileBase);
        public bool TryGetTileAt(Vector2 v, out TileInstance tile);
        public void DamageTileAt(Vector2 v, ushort damage);
    }
}