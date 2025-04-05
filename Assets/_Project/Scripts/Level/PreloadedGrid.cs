using Coimbra;
using Coimbra.Services;
using Core.Map;
using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Core.Level
{
    public class PreloadedGrid : Actor, IGridService
    {
        [SerializeField] private Tilemap _tilemap;

        protected override void OnInitialize()
        {
            base.OnInitialize();
            ServiceLocator.Set<IGridService>(this);
        }

        public bool HasTileAt(int x, int y)
        {
            return x >= 0 && y >= 0 && _tilemap.HasTile(new Vector3Int(x, y, 0));
        }

        public bool IsTileLoaded(int x, int y)
        {
            return x >= 0 && y >= 0;
        }

        public void DamageTileAt(int x, int y, ushort damage)
        {
            throw new System.NotImplementedException();
        }

        public void SetTileAt(int x, int y, TileBase tileBase)
        {
            throw new System.NotImplementedException();
        }

        public bool TryGetTileAt(int x, int y, out TileInstance tile)
        {
            throw new System.NotImplementedException();
        }
    }
}