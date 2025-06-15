using Core.Level;
using Core.Level.Dynamic;
using Core.Units;
using Core.Util;
using UnityEngine;

namespace Core.Interaction
{
    [CreateAssetMenu(menuName = "Core/Interactions/Green Grass")]
    public class GreenGrassInteractionEffectSO : InteractionEffectSO
    {
        public override void Execute(MonoBehaviour interactor, Vector2 worldPosition)
        {
            var foodService = ServiceLocatorUtilities.GetServiceAssert<IFoodService>();

            if (interactor is Ant ant && foodService.TryEat(worldPosition.RoundToInt()))
            {
                ant.Give(ItemSystem.Item.Leaf);
            }
        }
    }
}