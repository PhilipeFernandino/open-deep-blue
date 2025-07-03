using Core.Level;
using Core.Level.Dynamic;
using Core.Train;
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

            if (interactor is Ant ant
                && foodService.TryEat(worldPosition.RoundToInt())
                && ant.IsCarrying(ItemSystem.Item.None))
            {
                ant.GiveItem(ItemSystem.Item.Leaf);
                new AntEvent(AntEventType.GatherLeaf, ant).Invoke(this);
            }
        }
    }
}