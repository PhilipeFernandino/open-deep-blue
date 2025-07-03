using UnityEngine;

namespace Core.Level.Dynamic
{
    [CreateAssetMenu(menuName = "Core/Dynamic/Food Definition")]
    public class FoodDefinition : ScriptableObject
    {
        public float MaxFoodStore;
        public float FoodProduction;
    }
}