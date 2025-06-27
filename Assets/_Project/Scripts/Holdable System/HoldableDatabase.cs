using Coimbra;
using Core.ItemSystem;
using System;
using System.Collections.Generic;
using System.Text;
using TNRD;
using UnityEngine;

namespace Core.HoldableSystem
{
    [Serializable]
    public class HoldableItem
    {
        [field: SerializeField] public ItemSO ItemSO { get; private set; }
        [field: SerializeField] public SerializableInterface<IHoldable> Holdable { get; private set; }
    }

    [ProjectSettings("Core")]
    public class HoldableDatabase : ScriptableSettings
    {
        [SerializeField] private List<HoldableItem> _holdables = new();

        private Dictionary<ItemSO, SerializableInterface<IHoldable>> _holdablesDict = new();

        public bool TryGetHoldable(ItemSO itemSO, out IHoldable holdable)
        {
            if (_holdablesDict.TryGetValue(itemSO, out var holdableSI))
            {
                holdable = holdableSI.Value;
                return true;
            }
            Debug.Log($"{GetType()} Holdable for {itemSO} not found." +
                $"\nContains key: {_holdablesDict.ContainsKey(itemSO)}" +
                $"\nHoldables:\n{StringHoldables()}");
            holdable = null;
            return false;
        }

        protected override void OnLoaded()
        {
            _holdablesDict.Clear();

            foreach (var item in _holdables)
            {
                _holdablesDict.Add(item.ItemSO, item.Holdable);
            }

            Debug.Log($"{GetType()} - Loaded holdables\n{StringHoldables()}");
        }

        private string StringHoldables()
        {
            StringBuilder sb = new();
            foreach (var key in _holdablesDict.Keys)
            {
                sb.AppendLine($"{key}");
            }

            return sb.ToString();
        }
    }
}