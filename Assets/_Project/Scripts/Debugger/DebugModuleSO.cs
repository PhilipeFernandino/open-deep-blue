using System.Collections;
using UnityEngine;

namespace Core.Debugger
{
    public abstract class DebugModuleSO : ScriptableObject
    {
        [field: SerializeField] public string ModuleId { get; private set; }

        [field: SerializeField] public string ModuleTitle { get; private set; }

        [field: SerializeField] public bool IsActive { get; private set; } = true;

        [field: SerializeField] public Color Color { get; private set; } = default;

        public string DisplayText { get; protected set; } = "Awaiting data...";

        public abstract void UpdateData(object data);

        public virtual void ResetData()
        {
            DisplayText = "No data received.";
        }
    }
}