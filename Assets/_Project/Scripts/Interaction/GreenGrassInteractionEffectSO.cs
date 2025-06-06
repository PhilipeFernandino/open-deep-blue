using Core.Level;
using Core.Units;
using Core.Util;
using UnityEngine;

namespace Core.Interaction
{
    public class GreenGrassInteractionEffectSO : InteractionEffectSO
    {
        public override void Execute(MonoBehaviour interactor, Vector2 worldPosition)
        {
            IGridService gridService = ServiceLocatorUtilities.GetServiceAssert<IGridService>();
            gridService.DamageTileAt(worldPosition, 5);
            ((Ant)interactor).Give(ItemSystem.Item.Leaf);
        }
    }
}