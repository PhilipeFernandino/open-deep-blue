using Coimbra;
using Core.UI;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Core.Debugger
{
    public class DebugUI : UIDynamicCanvas
    {
        [Header("UI Prefabs and Containers")]
        [SerializeField] private TextMeshProUGUI _debugTextPrefab;
        [SerializeField] private Transform _textContainer;

        private Dictionary<string, TextMeshProUGUI> _uiElements = new();

        public void Initialize(List<DebugModuleSO> activeModules)
        {
            foreach (Transform child in _textContainer)
            {
                child.gameObject.Dispose(true);
            }

            _uiElements.Clear();

            foreach (var module in activeModules)
            {
                TextMeshProUGUI newTextElement = Instantiate(_debugTextPrefab, _textContainer);
                _uiElements.Add(module.ModuleId, newTextElement);
                UpdateModuleText(module);
            }
        }


        public void UpdateModuleText(DebugModuleSO module)
        {
            if (_uiElements.ContainsKey(module.ModuleId))
            {
                _uiElements[module.ModuleId].text =
                    $"<b><color=#{ColorUtility.ToHtmlStringRGB(module.Color)}>{module.ModuleTitle}:</color></b>" +
                    $"\n{module.DisplayText}";
            }
        }
    }
}