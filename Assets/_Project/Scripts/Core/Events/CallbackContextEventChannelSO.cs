using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Core.Events
{
    [CreateAssetMenu(menuName = "Core/Events/Input Action Callback Context Event Channel")]
    public class CallbackContextEventChannelSO : ScriptableObject
    {
        public event Action<InputAction.CallbackContext> OnEventRaised;

        public void RaiseEvent(InputAction.CallbackContext context)
        {
            OnEventRaised?.Invoke(context);
        }
    }
}