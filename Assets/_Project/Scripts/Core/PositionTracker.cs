using System.Collections;
using UnityEngine;

namespace Core.Events
{
    public class PositionTracker : MonoBehaviour
    {
        [SerializeField] private Vector2EventChannelSO _positionEventBus;

        private void Update()
        {
            _positionEventBus.RaiseEvent(transform.position);
        }
    }
}