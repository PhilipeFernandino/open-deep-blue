using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Core.Map
{
    public class TilemapDebug : MonoBehaviour
    {
        [SerializeField] private Tilemap _tilemap;
        [SerializeField] private TileBase _tileBase;

        public int x, y;

        [Button]
        private void AddTileAt()
        {
            _tilemap.SetTile(new Vector3Int(x, y, 0), _tileBase);
        }
    }
}
