using Core.Map;

namespace Core.Level.Dynamic
{
    public struct FungusData : IDynamicTileData
    {
        public float CurrentSaciation;
        public float CurrentFoodStore;
        public float CurrentHealth;
    }

    public struct FungusDefinition : IDynamicTileData
    {
        public float MaxFoodStore;
        public float MaxHealth;
        public float MaxSaciation;
        public float SaciationLost;
        public float LostHealthWhenStarved;
        public float FoodProduction;
    }
}