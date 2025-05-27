namespace Core.Map
{
    public enum Tile : ushort
    {
        None = 0,

        // Stones and ores
        BlueStone = 1,
        CopperOre = 2,
        SilverOre = 3,
        MoonOre = 4,
        MoonStone = 5,
        Amazonita = 6,

        GreenGrass = 101,
        Fluffy = 102,

        TreeRoot = 151,
        // Interactive --
        // Chests
        Chest = 1001,
        Bed = 1002,

        // Light Sources
        Torch = 1111,

        // Boss mark tiles
        AntQueenSpawn = 2001,
        AntQueenRoomTile = 2002,

    }
}