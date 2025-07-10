using System;
using UnityEngine;
using UnityEngine.Events;

namespace Core.Events
{
    [CreateAssetMenu(menuName = "Core/Events/Float Event Channel")]
    public class FloatEventChannelSO : DescriptionBaseSO
    {
        public event Action<float> OnEventRaised;

        public void RaiseEvent(float value)
        {
            if (OnEventRaised != null)
                OnEventRaised.Invoke(value);
        }
    }
}