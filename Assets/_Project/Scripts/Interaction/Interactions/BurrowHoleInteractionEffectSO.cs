using UnityEngine;

namespace Core.Interaction
{
    [CreateAssetMenu(menuName = "Core/Interactions/Burrow Hole")]
    public class BurrowHoleInteractionEffectSO : InteractionEffectSO
    {
        public override void Execute(MonoBehaviour interactor, Vector2 worldPosition)
        {
            if (interactor is Player.Player player)
            {
                Debug.Log($"Burrow hole interaction: {player}", this);
            }
        }
    }
}