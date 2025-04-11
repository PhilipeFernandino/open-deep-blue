using System.Collections;
using UnityEngine;

namespace Core.HoldableSystem
{
    [CreateAssetMenu(menuName = "Core/Equipment/AttackEquipmentAtts")]
    public class AttackEquipmentAttributes : EquipmentAttributes
    {
        [field: SerializeField] public UShortRangeAttribute Damage { get; private set; }
    }
}