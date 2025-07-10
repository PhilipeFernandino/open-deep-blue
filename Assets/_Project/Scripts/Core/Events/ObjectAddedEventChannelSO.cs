using System;
using UnityEngine;


namespace Core.Events
{
    [CreateAssetMenu(menuName = "Core/Events/Object Added Event Channel")]
    public class ObjectAddedEventChannelSO : ScriptableObject
    {
        public Action<object> ObjectAdded;
        public Action<object> ObjectRemoved;

        public void AddObject(object obj)
        {
            ObjectAdded?.Invoke(obj);
        }

        public void RemoveObject(object obj)
        {
            ObjectRemoved?.Invoke(obj);
        }
    }
}