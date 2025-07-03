using UnityEngine;

namespace Core.Level.Dynamic
{
    [CreateAssetMenu(menuName = "Core/Dynamic/Fungus Definition")]
    public class FungusDefinition : ScriptableObject
    {
        public float MaxFoodStore;
        public float MaxHealth;
        public float MaxSaciation;
        public float SaciationLost;
        public float LostHealthWhenStarved;
        public float FoodProduction;
        public float FoodProductionSaciationThreshold;
    }
}