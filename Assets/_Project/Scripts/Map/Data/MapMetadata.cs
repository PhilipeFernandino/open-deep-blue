using System.Collections.Generic;

namespace Core.Map
{
    public record MapMetadata(Tile[,] Tiles, Tile[,] BiomeTiles, List<PointOfInterest> PointsOfInterest, int Dimensions, string Name = "")
    {
        public int Dimensions { get; private set; } = Dimensions;
        public Tile[,] Tiles { get; private set; } = Tiles;
        public Tile[,] BiomeTiles { get; private set; } = BiomeTiles;
        public List<PointOfInterest> PointsOfInterest { get; private set; } = PointsOfInterest;
        public string Name { get; private set; } = Name;

    }
}