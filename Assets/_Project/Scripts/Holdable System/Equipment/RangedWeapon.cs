using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

namespace Core.HoldableSystem
{
    public class RangedWeapon : Equipment
    {
        [SerializeField] private Transform _projectileOrigin;
        [SerializeField] private Projectile _projectilePrefab;

        private ObjectPool<Projectile> _projectilePool;

        public RangedWeaponAttributes Attributes => (RangedWeaponAttributes)_attributes;

        protected override void UseBehavior(Vector2 worldPosition)
        {
            Projectile projectile = _projectilePool.Get();
            projectile.transform.position = _projectileOrigin.position;
            Vector2 dir = worldPosition - transform.position.XY();
            projectile.Setup(dir);
        }

        private void ReleaseCallback_EventHandler(Projectile projectile)
        {
            _projectilePool.Release(projectile);
        }

        protected override void Start()
        {
            base.Start();
            _projectilePool = new(() =>
            {
                Projectile projectile = Instantiate(_projectilePrefab, null);
                projectile.Create(Attributes, ReleaseCallback_EventHandler);
                return projectile;
            },
            defaultCapacity: (int)Attributes.UsesPerSecond * 15,
            maxSize: (int)Attributes.UsesPerSecond * 50);
        }
    }
}