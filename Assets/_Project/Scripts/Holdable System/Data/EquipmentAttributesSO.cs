using PrimeTween;
using System.Collections;
using UnityEngine;

namespace Core.HoldableSystem
{
    [CreateAssetMenu(menuName = "Core/Equipment")]
    public class EquipmentAttributes : ScriptableObject
    {
        [Tooltip("How many times the equipment can be used per second")]
        [SerializeField] public float UsesPerSecond { get; protected set; }
        [SerializeField] public float AngleOffset { get; protected set; }
        [SerializeField] public float DisableSRDelaySeconds { get; protected set; } = 0.1f;
        [SerializeField] public float TimeScaleOnBlink { get; protected set; } = 0.5f;
        [SerializeField] public float BlinkSeconds { get; protected set; } = .2f;
        [SerializeField] public HoldableUseEffect UseEffect { get; protected set; }
        [SerializeField] public ShakeSettings ShakeSettings { get; protected set; }
        [SerializeField] public TweenSettings<Vector3> RotTweenSettings { get; protected set; }
    }
}