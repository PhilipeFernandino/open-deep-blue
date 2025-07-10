using System;
using UnityEngine;

namespace Core.Events
{
    [CreateAssetMenu(menuName = "Core/Events/Bool Event Channel")]
    public class BoolEventChannelSO : DescriptionBaseSO
    {
        public event Action<bool> OnEventRaised;

        public void RaiseEvent(bool value)
        {
            if (OnEventRaised != null)
                OnEventRaised.Invoke(value);
        }
    }
}
