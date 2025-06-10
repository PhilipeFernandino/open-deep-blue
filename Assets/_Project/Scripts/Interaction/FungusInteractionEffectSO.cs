using Core.Level;
using Core.Level.Dynamic;
using Core.Units;
using Core.Util;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;

namespace Core.Interaction
{
    [CreateAssetMenu(menuName = "Core/Interactions/Fungus")]
    public class FungusInteractionEffectSO : InteractionEffectSO
    {
        public override void Execute(MonoBehaviour interactor, Vector2 worldPosition)
        {
            var gridService = ServiceLocatorUtilities.GetServiceAssert<IGridService>();
            var dynamicGridManager = ServiceLocatorUtilities.GetServiceAssert<IDynamicGridManager>();


            if (interactor is Ant ant)
            {
                if (ant.IsCarrying == ItemSystem.Item.Leaf)
                {
                    ModifyFungusData modification = (ref FungusData fungusData) =>
                    {
                        fungusData.CurrentSaciation += 10f;
                    };

                    bool success = dynamicGridManager.TryApplyFungusModification(worldPosition.RoundToInt(), modification);

                    if (success)
                    {
                        ant.Give(ItemSystem.Item.None);
                    }
                }
                else
                {
                    bool gotFood = dynamicGridManager.TryGetFungusFood(worldPosition.RoundToInt());
                    if (gotFood)
                    {
                        ant.Give(ItemSystem.Item.Fungus);
                    }
                }
            }
        }
    }
}