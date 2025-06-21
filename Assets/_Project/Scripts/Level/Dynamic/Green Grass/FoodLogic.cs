using UnityEngine;

namespace Core.Level.Dynamic
{
    public class FoodLogic
    {
        public void OnUpdate(ref FoodData data, FoodDefinition defData, Vector2Int position, IGridService grid)
        {
            data.CurrentFoodStore = Mathf.Min(
                defData.MaxFoodStore,
                data.CurrentFoodStore + (defData.FoodProduction * Time.fixedDeltaTime)
            );
        }
    }
}