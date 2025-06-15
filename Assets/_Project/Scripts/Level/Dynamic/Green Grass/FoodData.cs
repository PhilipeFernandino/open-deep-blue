namespace Core.Level.Dynamic
{
    public struct FoodData : IDynamicTileData
    {
        public float CurrentFoodStore;
    }

    public struct FoodDefinition
    {
        public float MaxFoodStore;
        public float FoodProduction;
    }
}