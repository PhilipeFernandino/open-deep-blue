using Coimbra;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Core.Debugger
{

    public class DebugUI : MonoBehaviour
    {
        [Header("UI Prefabs and Containers")]
        [Tooltip("A prefab for a single line of debug text (e.g., a TextMeshProUGUI object).")]
        [SerializeField] private TextMeshProUGUI _debugTextPrefab;

        [Tooltip("The parent transform where new text elements will be instantiated.")]
        [SerializeField] private Transform _textContainer;

        private Dictionary<string, TextMeshProUGUI> _uiElements = new();

        /// <summary>
        /// Creates the initial UI layout based on the currently active modules.
        /// </summary>
        public void Initialize(List<DebugModuleSO> activeModules)
        {
            foreach (Transform child in _textContainer)
            {
                child.gameObject.Dispose(true);
            }

            _uiElements.Clear();

            if (_debugTextPrefab == null)
            {
                Debug.LogError("DebugUI: Debug Text Prefab is not assigned!", this);
                return;
            }

            foreach (var module in activeModules)
            {
                TextMeshProUGUI newTextElement = Instantiate(_debugTextPrefab, _textContainer);
                _uiElements.Add(module.ModuleId, newTextElement);
                UpdateModuleText(module);
            }
        }

        /// <summary>
        /// Updates a specific module's text when new data is received.
        /// </summary>
        public void UpdateModuleText(DebugModuleSO module)
        {
            // Use the public ModuleId property for the dictionary key
            if (_uiElements.ContainsKey(module.ModuleId))
            {
                // Use the public properties (ModuleTitle, DisplayText)
                _uiElements[module.ModuleId].text =
                    $"<b><color=#{ColorUtility.ToHtmlStringRGB(module.Color)}>{module.ModuleTitle}:</color></b>" +
                    $"\n{module.DisplayText}";
            }
        }
    }
}