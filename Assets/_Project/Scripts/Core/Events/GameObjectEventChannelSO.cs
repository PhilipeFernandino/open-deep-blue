using System;
using UnityEngine;

namespace Core.Events
{
    [CreateAssetMenu(menuName = "Core/Events/GameObject Event Channel")]
    public class GameObjectEventChannelSO : DescriptionBaseSO
    {
        public event Action<GameObject> OnEventRaised;

        public void RaiseEvent(GameObject value)
        {
            if (OnEventRaised != null)
                OnEventRaised.Invoke(value);
        }
    }

}