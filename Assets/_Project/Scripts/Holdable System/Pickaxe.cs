using UnityEngine;

namespace Core.HoldableSystem
{
    public class Pickaxe : Holdable
    {
        public override (bool success, HoldableUseEffect effect) TryUse(Vector2 worldPosition)
        {
            return (false, HoldableUseEffect.None);
        }

    }
}