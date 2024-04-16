using UnityEngine;
using UnityEngine.Tilemaps;

namespace Systems.GridSystem
{
    public class TilemapToGrid : MonoBehaviour
    {
        [SerializeField] private Tilemap _tilemap;
        [SerializeField] private CustomGrid _customGrid;

        private void Start()
        {
            int gridSize = _customGrid.GridAxisRange;
            var boundsInt = new BoundsInt(-gridSize, -gridSize, 0, gridSize, gridSize, 1);
            var tiles = _tilemap.GetTilesBlock(boundsInt);

            foreach (var tile in tiles)
            {
                Debug.Log(tile);
            }
        }
    }
}