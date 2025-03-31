using Systems.Grid_System;
using UnityEngine;

namespace Core.HoldableSystem
{

    public class Pickaxe : Equipment
    {
        [SerializeField] protected new PickaxeAttributesSO _attributes;

        private IGridService _gridService;

        protected override void UseBehavior(Vector2 worldPosition)
        {
            _gridService.DamageTileAt(worldPosition, _attributes.Damage);
        }
    }
}