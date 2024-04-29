using UnityEngine;

namespace Core.HealthSystem.UI
{
    [CreateAssetMenu(menuName = "Core/UI/UI Hit Color Data")]
    public class UIHitColorData : ScriptableObject
    {
        [field: SerializeField] public Color NormalHitColor { get; private set; }
        [field: SerializeField] public Color CriticalHitColor { get; private set; }
        [field: SerializeField] public Color HealHitColor { get; private set; }

        public Color GetColor(AttackType attackType) =>
            attackType switch
            {
                AttackType.Damage => NormalHitColor,
                AttackType.Critical => CriticalHitColor,
                AttackType.Heal => HealHitColor,
                _ => NormalHitColor,
            };
    }
}