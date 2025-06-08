using UnityEngine;

namespace Core.HoldableSystem
{

    public class Pickaxe : Equipment
    {
        public PickaxeAttributesSO Attributes => (PickaxeAttributesSO)_attributes;

        protected override void UseBehavior(Vector2 worldPosition)
        {
            _gridService.DamageTileAt(worldPosition, Attributes.MiningStrength);
        }
    }
}