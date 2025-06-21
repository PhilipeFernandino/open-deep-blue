using Coimbra;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Core.Debugger
{
    public class DebugManager : Actor
    {

        [Header("Configuration")]
        [SerializeField] private DebugChannelSO _debugChannel;

        [SerializeField] private List<DebugModuleSO> _availableModules;

        [Header("Input")]
        [SerializeField] private InputActionReference _toggleActionReference;

        [Header("UI")]
        [SerializeField] private DebugUI _debugUI;

        public void ToggleDebugUI()
        {
            bool isNowActive = !_debugUI.gameObject.activeSelf;
            _debugUI.gameObject.SetActive(isNowActive);

            if (isNowActive)
            {
                _debugUI.Initialize(_availableModules.FindAll(m => m.IsActive));
            }
        }

        protected override void OnSpawn()
        {
            _debugChannel.OnEventRaised += HandleEvent;
            _toggleActionReference.action.Enable();
            _toggleActionReference.action.performed += OnToggleInput;

            _debugUI.gameObject.SetActive(false);
        }

        private void OnToggleInput(InputAction.CallbackContext context)
        {
            ToggleDebugUI();
        }

        private void HandleEvent(string moduleId, object data)
        {
            foreach (var module in _availableModules)
            {
                if (module.IsActive && module.ModuleId == moduleId)
                {
                    module.UpdateData(data);
                    if (_debugUI != null && _debugUI.gameObject.activeInHierarchy)
                    {
                        _debugUI.UpdateModuleText(module);
                    }
                    return;
                }
            }
        }
    }
}
