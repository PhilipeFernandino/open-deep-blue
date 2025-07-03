using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Units
{
    public class Blackboard<T> where T : Enum
    {
        public Dictionary<T, object> _data;

        public void Set<K>(T key, K value)
        {
            _data[key] = value;
        }

        public K Get<K>(T key)
        {
            if (_data.TryGetValue(key, out object value))
            {
                return (K)value;
            }
            return default;
        }

        public bool HasKey(T key)
        {
            return _data.ContainsKey(key);
        }

        public bool TryGet<K>(T key, out K value)
        {
            if (_data.TryGetValue(key, out object obj))
            {

                value = (K)obj;
                return true;
            }
            value = default;
            return false;
        }
    }
}