using Core.FSM;
using Core.HoldableSystem;
using UnityEngine;

namespace Core.Player
{
    public class UsingEquipmentEnterStateData : IEnterStateData
    {
        public readonly Vector2 Input;
        public readonly HoldableUseEffect UseEffect;

        public UsingEquipmentEnterStateData(Vector2 input, HoldableUseEffect equipmentUseEffect)
        {
            Input = input;
            UseEffect = equipmentUseEffect;
        }
    }
}
