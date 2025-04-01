using PrimeTween;
using System.Collections;
using UnityEngine;
using UnityEngine.Assertions.Must;

namespace Core.HoldableSystem
{
    [CreateAssetMenu(menuName = "Core/Equipment")]
    public class EquipmentAttributes : ScriptableObject
    {
        [Tooltip("How many times the equipment can be used per second")]
        [field: SerializeField] public float UsesPerSecond { get; protected set; } = 1;
        [field: SerializeField] public float AngleOffset { get; protected set; } = 0f;
        [field: SerializeField] public float DisableSRDelaySeconds { get; protected set; } = 0.1f;
        [field: SerializeField] public float TimeScaleOnBlink { get; protected set; } = 0.5f;
        [field: SerializeField] public float BlinkSeconds { get; protected set; } = .2f;
        [field: SerializeField] public HoldableUseEffect UseEffect { get; protected set; } = HoldableUseEffect.None;
        [field: SerializeField] public ShakeSettings ShakeSettings { get; protected set; } = default;
        [field: SerializeField] public TweenSettings<Vector3> RotTweenSettings { get; protected set; } = default;
    }
}