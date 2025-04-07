using System.Collections;
using UnityEngine;

namespace Core.HoldableSystem
{
    [CreateAssetMenu(menuName = "Core/Equipment/LightSourceAtts")]
    public class LightSourceEquipmentAttributesSO : EquipmentAttributes
    {
        [field: SerializeField] public int Intensity { get; private set; }
    }
}