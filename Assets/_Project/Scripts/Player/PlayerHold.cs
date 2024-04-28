using UnityEngine;

namespace Core.Units
{
    public class PlayerHold : MonoBehaviour
    {
        [SerializeField] private Equipment _equipment;

        public void TryUseEquipment(Vector2 worldPosition)
        {
            _equipment.TryUse(worldPosition);
        }
    }
}