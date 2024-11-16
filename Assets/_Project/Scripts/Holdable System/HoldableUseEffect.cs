using System;
using UnityEngine;

namespace Core.HoldableSystem
{
    [Serializable]
    public struct HoldableUseEffect
    {
        [field: SerializeField] public float LockDuration { get; private set; }

        [field: SerializeField] public float AddedImpulseSpeed { get; private set; }
        [field: SerializeField] public float AddedImpulseDuration { get; private set; }


        public readonly Vector2 AddedVelocity;

        public static HoldableUseEffect None => new HoldableUseEffect();

        public HoldableUseEffect(float lockDuration, float addedImpulseSpeed, float addedImpulseDuration, Vector2 addedVelocity)
        {
            LockDuration = lockDuration;
            AddedVelocity = addedVelocity;
            AddedImpulseSpeed = addedImpulseSpeed;
            AddedImpulseDuration = addedImpulseDuration;
        }

        public readonly HoldableUseEffect Create(Vector2 addedVelocity)
        {
            return new HoldableUseEffect(LockDuration, AddedImpulseSpeed, AddedImpulseDuration, addedVelocity);
        }
    }
}