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

        // Interactive 
        Chest = 1001,

        // Boss mark tiles
        AntQueenSpawn = 2001,
        AntQueenRoomTile = 2002,

    }
}