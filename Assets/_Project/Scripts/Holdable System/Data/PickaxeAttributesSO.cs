using UnityEngine;

namespace Core.HoldableSystem
{
    [CreateAssetMenu(menuName = "Core/Equipment/PickaxeAtts")]
    public class PickaxeAttributesSO : EquipmentAttributes
    {
        [field: SerializeField] public ushort MiningStrength { get; private set; }
    }
}