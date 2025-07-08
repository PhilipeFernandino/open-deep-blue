using Core;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseMappingSO<TKey, TValue> : ScriptableObject
{
    [SerializeField]
    protected List<SerializableKeyValuePair<TKey, TValue>> _serializableData = new List<SerializableKeyValuePair<TKey, TValue>>();

    protected Dictionary<TKey, TValue> _runtimeData;

    protected virtual void OnEnable()
    {
        InitializeRuntimeData();
    }

    protected virtual void OnValidate()
    {
        InitializeRuntimeData();
    }

    protected void InitializeRuntimeData()
    {
        if (_runtimeData == null)
        {
            _runtimeData = new Dictionary<TKey, TValue>();
        }
        else
        {
            _runtimeData.Clear();
        }

        foreach (var pair in _serializableData)
        {
            if (!_runtimeData.ContainsKey(pair.Key)) // Prevent duplicate key errors
            {
                _runtimeData.Add(pair.Key, pair.Value);
            }
            else
            {
                Debug.LogWarning($"Duplicate key '{pair.Key}' found in ScriptableObject {name}. Skipping duplicate.");
            }
        }
    }

    /// <summary>
    /// Attempts to get a value associated with the specified key.
    /// </summary>
    /// <param name="key">The key of the value to get.</param>
    /// <param name="value">When this method returns, contains the value associated with the specified key, if the key is found; otherwise, the default value for the type of the value parameter.</param>
    /// <returns>True if the dictionary contains an element with the specified key; otherwise, false.</returns>
    public bool TryGetValue(TKey key, out TValue value)
    {
        // Ensure the runtime dictionary is initialized before trying to access it.
        // This can happen if you try to access the SO before OnEnable/OnValidate.
        if (_runtimeData == null)
        {
            InitializeRuntimeData();
        }
        return _runtimeData.TryGetValue(key, out value);
    }

    /// <summary>
    /// Checks if the dictionary contains a specific key.
    /// </summary>
    /// <param name="key">The key to locate.</param>
    /// <returns>True if the dictionary contains an element with the specified key; otherwise, false.</returns>
    public bool ContainsKey(TKey key)
    {
        if (_runtimeData == null)
        {
            InitializeRuntimeData();
        }
        return _runtimeData.ContainsKey(key);
    }

    /// <summary>
    /// Gets the number of key/value pairs contained in the dictionary.
    /// </summary>
    public int Count
    {
        get
        {
            if (_runtimeData == null)
            {
                InitializeRuntimeData();
            }
            return _runtimeData.Count;
        }
    }

    /// <summary>
    /// Gets the value associated with the specified key.
    /// Throws a KeyNotFoundException if the key does not exist.
    /// </summary>
    /// <param name="key">The key of the value to get or set.</param>
    /// <returns>The value associated with the specified key.</returns>
    public TValue this[TKey key]
    {
        get
        {
            if (_runtimeData == null)
            {
                InitializeRuntimeData();
            }
            return _runtimeData[key];
        }
    }
}