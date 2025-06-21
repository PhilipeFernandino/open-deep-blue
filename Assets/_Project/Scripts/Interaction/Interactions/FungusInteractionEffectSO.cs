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
            var fungusService = ServiceLocatorUtilities.GetServiceAssert<IFungusService>();

            Debug.Log($"Trying to interact with fungus", this);

            if (interactor is Ant ant)
            {
                Debug.Log($"Is ant", this);

                if (ant.IsCarrying(ItemSystem.Item.Leaf))
                {
                    ModifyFungusData modification = (ref FungusData fungusData) =>
                    {
                        fungusData.CurrentSaciation += 10f;
                    };

                    bool success = fungusService.TryApplyModification(worldPosition.RoundToInt(), modification);

                    if (success)
                    {
                        ant.Give(ItemSystem.Item.None);
                    }
                }
                else
                {
                    bool gotFood = fungusService.TryTakeFungusFood(worldPosition.RoundToInt());
                    Debug.Log($"Got food: {gotFood}", this);
                    if (gotFood)
                    {
                        ant.Give(ItemSystem.Item.Fungus);
                    }
                }
            }
        }
    }
}