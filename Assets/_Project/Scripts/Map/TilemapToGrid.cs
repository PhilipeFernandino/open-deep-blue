using Core.Level;
using Core.Map;
using Core.Util;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Core.Map
{
    [System.Serializable]
    public class TileToGrid
    {
        public TileBase TileBase;
        public Tile Tile;
    }

    public class TilemapToGrid : MonoBehaviour
    {
        [SerializeField] private List<TileToGrid> _tiles = new List<TileToGrid>();
        [SerializeField] private Tilemap _tilemap;
        [SerializeField] private int _dimensions;

        private Dictionary<TileBase, Tile> _tilesDict;

        private IGridService _gridService;

        private void Start()
        {
            _tilesDict = new();
            _gridService = ServiceLocatorUtilities.GetServiceAssert<IGridService>();

            foreach (var tile in _tiles)
            {
                _tilesDict.Add(tile.TileBase, tile.Tile);
            }
            Tile[,] tiles = new Tile[_dimensions, _dimensions];
            Tile[,] biomeTiles = new Tile[_dimensions, _dimensions];

            bool found = false;
            for (int x = 0; x < _dimensions; x++)
            {
                for (int y = 0; y < _dimensions; y++)
                {
                    var tilebase = _tilemap.GetTile(new(x, y, 0));
                    if (tilebase)
                    {
                        if (!found)
                        {
                            found = true;
                            Debug.Log($"found tile {tilebase} at: ({x}, {y}), set {_tilesDict[tilebase]}");
                        }
                        tiles[x, y] = _tilesDict[tilebase];
                    }
                }
            }

            if (!found)
            {
                throw new System.Exception();
            }

            var map = new MapMetadata(tiles, biomeTiles, new List<PointOfInterest>(), _dimensions);
            new MapMetadataGeneratedEvent(map).Invoke(this);
        }
    }

}