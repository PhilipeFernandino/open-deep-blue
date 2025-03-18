using System.Collections.Generic;

namespace Core.Map
{
    public record MapMetadata(Tile[,] Tiles, List<PointOfInterest> PointsOfInterest, int Dimensions)
    {
        public int Dimensions { get; private set; } = Dimensions;
        public Tile[,] Tiles { get; private set; } = Tiles;
        public List<PointOfInterest> PointsOfInterest { get; private set; } = PointsOfInterest;
    }
}