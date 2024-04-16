namespace Systems.GridSystem
{
    public struct TileData
    {
        public GroundTile GroundTile { get; set; }
        public IPlaceable Placeable { get; set; } // will be boxed

        public static TileData None => new() { GroundTile = GroundTile.None };
    }

    public enum GroundTile
    {
        None,
        Dirt,
        Stone,
    }

    public interface IPlaceable
    {

    }
}