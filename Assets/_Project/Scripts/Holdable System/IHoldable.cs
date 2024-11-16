using UnityEngine;

namespace Core.HoldableSystem
{
    public interface IHoldable
    {
        public (bool success, HoldableUseEffect effect) TryUse(Vector2 worldPosition);
    }
}