using UnityEngine;

namespace Core.Interaction
{
    public abstract class InteractionEffectSO : ScriptableObject
    {
        public abstract void Execute(MonoBehaviour interactor, Vector2 worldPosition);
    }
}