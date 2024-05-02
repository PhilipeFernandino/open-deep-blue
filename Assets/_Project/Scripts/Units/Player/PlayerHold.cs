using UnityEngine;

namespace Core.Player
{
    public class PlayerHold : MonoBehaviour
    {
        [SerializeField] private Equipment _equipment;

        public (bool success, EquipmentUseEffect effect) TryUseEquipment(Vector2 worldPosition)
        {
            return _equipment.TryUse(worldPosition);
        }
    }
}