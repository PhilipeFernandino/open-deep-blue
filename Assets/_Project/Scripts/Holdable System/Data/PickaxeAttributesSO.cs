using UnityEngine;

namespace Core.HoldableSystem
{
    [CreateAssetMenu(menuName = "Core/Equipment/Pickaxe")]
    public class PickaxeAttributesSO : EquipmentAttributes
    {
        [field: SerializeField] public ushort MiningStrength { get; private set; }
    }
}