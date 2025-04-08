using System.Collections;
using UnityEngine;

namespace Core.EventBus
{
    public class PositionTracker : MonoBehaviour
    {
        [SerializeField] private PositionEventBus _positionEventBus;

        private void Update()
        {
            _positionEventBus.Position = transform.position;
        }
    }
}