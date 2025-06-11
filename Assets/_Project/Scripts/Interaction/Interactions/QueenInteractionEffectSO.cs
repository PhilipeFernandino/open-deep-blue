using Core.Level;
using Core.Level.Dynamic;
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

            if (interactor is Ant ant)
            {
                if (ant.IsCarrying == ItemSystem.Item.Fungus)
                {
                    bool success = queenService.TryFeedTheQueen(worldPosition.RoundToInt(), 5f);

                    if (success)
                    {
                        ant.Give(ItemSystem.Item.None);
                    }
                }
            }
        }
    }
}