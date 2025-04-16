using UnityEngine;

namespace Core.HoldableSystem
{
    [CreateAssetMenu(menuName = "Core/Equipment/Pickaxe Attributes")]
    public class PickaxeAttributesSO : EquipmentAttributes
    {
        [field: SerializeField] public uint MiningStrength { get; private set; }
    }
}