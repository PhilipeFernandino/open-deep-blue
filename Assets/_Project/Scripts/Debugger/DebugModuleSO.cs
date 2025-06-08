using System.Collections;
using UnityEngine;

namespace Core.Debugger
{
    public abstract class DebugModuleSO : ScriptableObject
    {
        [field: Tooltip("The unique ID for this module. Must match the ID used when raising the event.")]
        [field: SerializeField] public string ModuleId { get; private set; }

        [field: Tooltip("The title displayed in the debug UI for this module.")]
        [field: SerializeField] public string ModuleTitle { get; private set; }

        [field: Tooltip("Is this module currently active and listening for events?")]
        [field: SerializeField] public bool IsActive { get; private set; } = true;

        [field: Tooltip("The color for the module title")]
        [field: SerializeField] public Color Color { get; private set; } = default;

        // This property holds the final, formatted string for display.
        // It has a public getter but can only be set by this class or its children.
        public string DisplayText { get; protected set; } = "Awaiting data...";

        /// <summary>
        /// Processes the raw data from the DebugChannel into a human-readable string.
        /// This method is implemented by each specific debug module.
        /// </summary>
        /// <param name="data">The data payload from the DebugChannel.</param>
        public abstract void UpdateData(object data);

        /// <summary>
        /// Resets the display text to a default state when the system starts or is reset.
        /// </summary>
        public virtual void ResetData()
        {
            DisplayText = "No data received.";
        }
    }
}