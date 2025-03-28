using AYellowpaper.SerializedCollections;
using Coimbra;
using Core.ItemSystem;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using TNRD;
using UnityEngine;

namespace Core.HoldableSystem
{
    [ProjectSettings("Game Settings")]
    public class HoldableDatabase : ScriptableSettings
    {
        [SerializeField] private SerializedDictionary<ItemSO, SerializableInterface<IHoldable>> _holdables;

        public bool TryGetHoldable(ItemSO itemSO, out IHoldable holdable)
        {
            if (_holdables.TryGetValue(itemSO, out var holdableSI))
            {
                holdable = holdableSI.Value;
                return true;
            }
            Debug.Log($"{GetType()} Holdable for {itemSO} not found." +
                $"\nContains key: {_holdables.ContainsKey(itemSO)}" +
                $"\nHoldables:\n{StringHoldables()}");
            holdable = null;
            return false;
        }

        protected override void OnLoaded()
        {
            base.OnLoaded();
            _holdables = new(_holdables);
            Debug.Log($"{GetType()} - Loaded holdables\n{StringHoldables()}");
        }

        private string StringHoldables()
        {
            StringBuilder sb = new();
            foreach (var (key, value) in _holdables)
            {
                sb.AppendLine($"{key}: {value.Value}");
            }

            return sb.ToString();
        }
    }
}