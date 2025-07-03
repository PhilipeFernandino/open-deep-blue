using Coimbra;
using Core.Level;
using Core.Level.Dynamic;
using Core.Train;
using Core.Units;
using Core.Util;
using UnityEngine;

namespace Core.Interaction
{
    [CreateAssetMenu(menuName = "Core/Interactions/Queen")]
    public class QueenInteractionEffectSO : InteractionEffectSO
    {
        public override void Execute(MonoBehaviour interactor, Vector2 worldPosition)
        {
            var queenService = ServiceLocatorUtilities.GetServiceAssert<IQueenService>();
            var colonyEconomySettings = ScriptableSettings.GetOrFind<ColonyEconomySettings>();

            if (interactor is Ant ant)
            {
                if (ant.Carrying == ItemSystem.Item.Fungus)
                {
                    bool success = queenService.TryFeedTheQueen(worldPosition.RoundToInt(), colonyEconomySettings.FungusFeedAntsAmount);

                    if (success)
                    {
                        ant.GiveItem(ItemSystem.Item.None);
                        new AntEvent(AntEventType.FeedQueen, ant).Invoke(this);
                    }
                }
            }
        }
    }
}