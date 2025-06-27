using System;
using UnityEngine;

namespace Core.Debugger
{

    [CreateAssetMenu(fileName = "DebugChannel", menuName = "Core/Debugger/Debug Channel")]
    public class DebugChannelSO : ScriptableObject
    {
        public event Action<string, object> OnEventRaised;

        [System.Diagnostics.Conditional(conditionString: "DEBUG")]
        public void RaiseEvent(string moduleId, object data)
        {
            OnEventRaised?.Invoke(moduleId, data);
        }
    }
}
