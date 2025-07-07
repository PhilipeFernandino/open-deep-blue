using Coimbra;
using Core.Level;
using Core.Level.Dynamic;
using Core.RL;
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
            var colonyEconomySettings = ScriptableSettings.GetOrFind<ColonyEconomySettings>();


            if (interactor is Ant ant)
            {
                if (ant.IsCarrying(ItemSystem.Item.Leaf))
                {
                    ModifyFungusData modification = (ref FungusData fungusData) =>
                    {
                        fungusData.CurrentSaciation += colonyEconomySettings.LeafFeedFungusAmount;
                    };

                    bool success = fungusService.TryApplyModification(worldPosition.RoundToInt(), modification);

                    if (success)
                    {
                        ant.GiveItem(ItemSystem.Item.None);
                        new AntEvent(AntEventType.FeedFungus, ant).Invoke(this);
                    }
                }
                else
                {
                    bool gotFood = fungusService.TryTakeFungusFood(worldPosition.RoundToInt());
                    if (gotFood)
                    {
                        ant.GiveItem(ItemSystem.Item.Fungus);
                        new AntEvent(AntEventType.GatherFungus, ant).Invoke(this);
                    }
                }
            }
        }
    }
}