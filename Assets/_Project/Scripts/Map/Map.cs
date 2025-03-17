using System.Collections.Generic;

namespace Core.Map
{
    public class Map
    {
        public int Dimensions { get; private set; }
        public Tile[,] Tiles { get; private set; }
        public List<PointOfInterest> PointsOfInterest { get; private set; }


        public Map(Tile[,] tiles, List<PointOfInterest> pointsOfInterest, int dimensions)
        {
            Tiles = tiles;
            PointsOfInterest = pointsOfInterest;
            Dimensions = dimensions;
        }
    }
}