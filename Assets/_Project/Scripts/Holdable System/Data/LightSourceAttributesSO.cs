using System.Collections;
using UnityEngine;

namespace Core.HoldableSystem
{
    [CreateAssetMenu(menuName = "Core/Equipment/Light Source Attributes")]
    public class LightSourceAttributesSO : EquipmentAttributes
    {
        [field: SerializeField] public uint Intensity { get; private set; }
    }
}