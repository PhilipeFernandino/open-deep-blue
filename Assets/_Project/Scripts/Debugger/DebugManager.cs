using Coimbra;
using Coimbra.Services;
using Core.Debugger;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Core.Debugger
{
    public class DebugManager : Actor
    {

        [Header("Configuration")]
        [Tooltip("The central channel for all debug communications.")]
        [SerializeField] private DebugChannelSO _debugChannel;

        [Tooltip("A list of all possible debug modules the system can display.")]
        [SerializeField] private List<DebugModuleSO> _availableModules;

        [Header("Input")]
        [Tooltip("Reference to the Input Action used to toggle the debug UI.")]
        [SerializeField] private InputActionReference _toggleActionReference;

        [Header("UI")]
        [Tooltip("Reference to the script that controls the UI panel.")]
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
