using UnityEngine;

namespace Core.Interaction
{
    [CreateAssetMenu(menuName = "Core/Interactions/Chest Interaction")]
    public class ChestInteractionEffectSO : InteractionEffectSO
    {
        public override void Execute(MonoBehaviour interactor, Vector2 worldPosition)
        {
            if (interactor is Player.Player player)
            {
                Debug.Log($"Chest interaction: {player}", this);
            }
        }
    }
}