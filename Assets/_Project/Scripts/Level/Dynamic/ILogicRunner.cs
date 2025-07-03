using Core.Map;
using UnityEngine;

namespace Core.Level.Dynamic
{
    public interface ILogicRunner
    {
        public object GetData(Vector2Int v);
        public void HandleTileChanged(int x, int y, TileInstance newTile);
        public void FixedUpdate();

        public object GetData(int x, int y)
        {
            Vector2Int v2 = new(x, y);
            return GetData(v2);
        }

        public object GetData(Vector2 v)
        {
            var (x, y) = v.RoundToIntTuple();
            return GetData(x, y);
        }

    }
}