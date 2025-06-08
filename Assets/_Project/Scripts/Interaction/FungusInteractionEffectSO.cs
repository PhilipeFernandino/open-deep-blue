using Core.Level;
using Core.Units;
using Core.Util;
using UnityEngine;

namespace Core.Interaction
{
    [CreateAssetMenu(menuName = "Core/Interactions/Fungus")]
    public class FungusInteractionEffectSO : InteractionEffectSO
    {
        public override void Execute(MonoBehaviour interactor, Vector2 worldPosition)
        {
            IGridService gridService = ServiceLocatorUtilities.GetServiceAssert<IGridService>();
            gridService.DamageTileAt(worldPosition, -5f);
            ((Ant)interactor).Give(ItemSystem.Item.Leaf);
        }
    }
}