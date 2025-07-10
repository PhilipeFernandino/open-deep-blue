using System;
using UnityEngine;

namespace Core.Events
{
    [CreateAssetMenu(menuName = "Core/Events/Int Event Channel")]
    public class IntEventChannelSO : DescriptionBaseSO
    {
        public event Action<int> OnEventRaised;

        public void RaiseEvent(int value)
        {
            if (OnEventRaised != null)
                OnEventRaised.Invoke(value);
        }
    }
}
