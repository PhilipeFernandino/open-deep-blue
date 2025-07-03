using UnityEngine;

namespace Core.HoldableSystem
{
    public abstract class Holdable : MonoBehaviour, IHoldable
    {
        public abstract (bool success, HoldableUseEffect effect) TryUse(Vector2 worldPosition);
    }
}