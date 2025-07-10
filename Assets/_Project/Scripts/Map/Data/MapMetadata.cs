using System.Collections.Generic;
using UnityEngine;

namespace Core.Map
{
    public record MapMetadata(Tile[,] Tiles, Tile[,] BiomeTiles, List<PointOfInterest> PointsOfInterest, int Dimensions, string Name = "")
    {
        public int Dimensions { get; private set; } = Dimensions;
        public Tile[,] Tiles { get; private set; } = Tiles;
        public Tile[,] BiomeTiles { get; private set; } = BiomeTiles;
        public List<PointOfInterest> PointsOfInterest { get; private set; } = PointsOfInterest;
        public string Name { get; private set; } = Name;

        public void SetTile(int x, int y, Tile tile)
        {
            Tiles[x, y] = tile;
        }

        public (bool found, Vector2Int position) FirstTile(Tile tile)
        {
            for (int i = 0; i < Dimensions; i++)
            {
                for (int j = 0; j < Dimensions; j++)
                {
                    Tile compareTile = Tiles[i, j];
                    if (compareTile == tile)
                    {
                        return (true, new(i, j));
                    }
                }
            }

            return (false, default);
        }

        public void ListPositions(Tile tile, List<Vector2Int> listPositions)
        {
            listPositions.Clear();

            for (int i = 0; i < Dimensions; i++)
            {
                for (int j = 0; j < Dimensions; j++)
                {
                    Tile compareTile = Tiles[i, j];
                    if (compareTile == tile)
                    {
                        listPositions.Add(new(i, j));
                    }
                }
            }
        }

        public void RemoveAll(Tile tile)
        {
            for (int i = 0; i < Dimensions; i++)
            {
                for (int j = 0; j < Dimensions; j++)
                {
                    Tile compareTile = Tiles[i, j];
                    if (compareTile == tile)
                    {
                        Tiles[i, j] = Tile.None;
                    }
                }
            }
        }
    }
}