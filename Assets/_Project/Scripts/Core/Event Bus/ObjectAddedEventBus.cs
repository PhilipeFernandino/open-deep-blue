using System;
using System.Collections;
using UnityEngine;


namespace Core.EventBus
{
    [CreateAssetMenu(menuName = "Core/EventBus/ObjectAddedEventBus")]
    public class ObjectAddedEventBus : ScriptableObject
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