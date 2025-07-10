using System;
using UnityEngine;

namespace Core.Events
{
    [CreateAssetMenu(menuName = "Core/Events/Void Event Channel")]
    public class VoidEventChannelSO : DescriptionBaseSO
    {
        public event Action OnEventRaised;

        public void RaiseEvent()
        {
            if (OnEventRaised != null)
                OnEventRaised.Invoke();
        }
    }
}


