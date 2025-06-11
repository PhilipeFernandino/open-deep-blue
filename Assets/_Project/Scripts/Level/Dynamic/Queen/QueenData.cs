namespace Core.Level.Dynamic
{
    public struct QueenData : IDynamicTileData
    {
        public float CurrentSaciation;
        public float CurrentHealth;
        public float CurrentPregnancyPercentage;
    }

    public struct QueenDefinition
    {
        public float MaxHealth;
        public float MaxSaciation;
        public float SaciationLost;
        public float LostHealthWhenStarved;
        public float PregnancyRate;
        public int BroodPerLaying;
    }
}