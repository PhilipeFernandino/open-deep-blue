using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Core.Map
{
    [CreateAssetMenu(fileName = "Tilemap Asset", menuName = "Core/Map/Tilemap Asset")]
    public class TilemapAsset : ScriptableObject
    {
        [SerializeField] private int _dimensions;

        [HideInInspector, SerializeField] private List<Tile> _tiles;
        [HideInInspector, SerializeField] private List<Tile> _biomeTiles;

        public int Dimensions => _dimensions;

        public Tile[,] Tiles
        {
            get { return To2DArray(_tiles); }
        }

        public Tile[,] BiomeTiles
        {
            get { return To2DArray(_biomeTiles); }
        }

        public void PopulateFrom(MapMetadata mapMetadata)
        {
            _dimensions = mapMetadata.Dimensions;
            _tiles = new List<Tile>(mapMetadata.Tiles.Cast<Tile>().ToArray());
            _biomeTiles = new List<Tile>(mapMetadata.Tiles.Cast<Tile>().ToArray());
        }

        private Tile[,] To2DArray(List<Tile> flatList)
        {
            if (flatList == null || flatList.Count != _dimensions * _dimensions)
                return new Tile[_dimensions, _dimensions];

            var newArray = new Tile[_dimensions, _dimensions];
            for (int i = 0; i < flatList.Count; i++)
            {
                int x = i / _dimensions;
                int y = i % _dimensions;
                newArray[x, y] = flatList[i];
            }
            return newArray;
        }
    }
}