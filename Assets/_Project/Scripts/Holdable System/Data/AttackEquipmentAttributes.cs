using System.Collections;
using UnityEngine;

namespace Core.HoldableSystem
{
    [CreateAssetMenu(menuName = "Core/Equipment/AttackEquipmentAtts")]
    public class AttackEquipmentAttributes : EquipmentAttributes
    {
        [field: SerializeField] public AttributeRange Damage { get; private set; }
    }
}