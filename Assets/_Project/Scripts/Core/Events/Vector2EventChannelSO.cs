using System;
using UnityEngine;

namespace Core.Events
{
    [CreateAssetMenu(menuName = "Core/Events/Vector2 Event Channel")]
    public class Vector2EventChannelSO : ScriptableObject
    {
        public event Action<Vector2> OnEventRaised;

        public void RaiseEvent(Vector2 position)
        {
            OnEventRaised?.Invoke(position);
        }
    }

}