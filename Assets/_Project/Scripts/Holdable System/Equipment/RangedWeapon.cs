using System.Collections;
using UnityEngine;

namespace Core.HoldableSystem
{
    public class RangedWeapon : Equipment
    {
        [SerializeField] private Transform _projectileOrigin;
        [SerializeField] private Projectile _projectilePrefab;
        public RangedWeaponAttributes Attributes => (RangedWeaponAttributes)_attributes;

        protected override void UseBehavior(Vector2 worldPosition)
        {
            Projectile projectile = Instantiate(_projectilePrefab, _projectileOrigin);
            Vector2 dir = worldPosition - transform.position.XY();
            projectile.Setup(Attributes, dir);
        }
    }
}