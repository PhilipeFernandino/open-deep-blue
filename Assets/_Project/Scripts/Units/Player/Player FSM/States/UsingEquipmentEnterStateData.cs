using Core.FSM;
using UnityEngine;

namespace Core.Player
{
    public class UsingEquipmentEnterStateData : IEnterStateData
    {
        public readonly Vector2 Input;
        public readonly EquipmentUseEffect UseEffect;

        public UsingEquipmentEnterStateData(Vector2 input, EquipmentUseEffect equipmentUseEffect)
        {
            Input = input;
            UseEffect = equipmentUseEffect;
        }
    }
}
