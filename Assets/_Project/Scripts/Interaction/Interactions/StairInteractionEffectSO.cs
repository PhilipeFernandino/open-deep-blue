using UnityEngine;

namespace Core.Interaction
{
    [CreateAssetMenu(menuName = "Core/Interactions/Stair Interaction")]
    public class StairInteractionEffectSO : InteractionEffectSO
    {
        public override void Execute(MonoBehaviour interactor, Vector2 worldPosition)
        {
            if (interactor is Player.Player player)
            {
                Debug.Log($"Stair interaction: {player}", this);
            }
        }
    }
}