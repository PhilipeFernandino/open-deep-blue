using Core.Level;
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
            IGridService gridService = ServiceLocatorUtilities.GetServiceAssert<IGridService>();

            if (interactor is Ant ant && ant.IsCarrying == ItemSystem.Item.None)
            {
                gridService.DamageTileAt(worldPosition, 5);
                ant.Give(ItemSystem.Item.Leaf);
            }
        }
    }
}