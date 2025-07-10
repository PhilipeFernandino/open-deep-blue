using Core.Events;
using Core.UI;
using UnityEngine;

namespace Core.ItemSystem
{
    [RequireComponent(typeof(UIDynamicCanvas))]
    public class InventoryInputHandler : MonoBehaviour
    {
        [SerializeField] private VoidEventChannelSO _toggleEventChannel;

        private UIDynamicCanvas _dynamicCanvas;

        private void Awake()
        {
            _dynamicCanvas = GetComponent<UIDynamicCanvas>();
        }

        private void OnEnable()
        {
            _toggleEventChannel.OnEventRaised += OnToggle;
        }

        private void OnDisable()
        {
            _toggleEventChannel.OnEventRaised -= OnToggle;
        }

        private void OnToggle()
        {
            _dynamicCanvas.ToggleSelf();
        }
    }
}