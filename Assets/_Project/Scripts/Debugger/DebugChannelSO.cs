using System;
using UnityEngine;

namespace Core.Debugger
{

    [CreateAssetMenu(fileName = "DebugChannel", menuName = "Core/Debugger/Debug Channel")]
    public class DebugChannelSO : ScriptableObject
    {
        public event Action<string, object> OnEventRaised;

        /// <summary>
        /// A game system calls this method to send its debug data.
        /// </summary>
        /// <param name="moduleId">A unique identifier for the data type (e.g., "GridInfo", "PlayerPosition").</param>
        /// <param name="data">The actual debug information to be displayed.</param>
        public void RaiseEvent(string moduleId, object data)
        {
            OnEventRaised?.Invoke(moduleId, data);
        }
    }
}
