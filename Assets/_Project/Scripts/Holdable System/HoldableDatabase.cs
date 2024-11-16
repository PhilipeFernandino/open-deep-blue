using AYellowpaper.SerializedCollections;
using Coimbra;
using Core.ItemSystem;
using System.Collections.Generic;
using TNRD;
using UnityEngine;

namespace Core.HoldableSystem
{
    [ProjectSettings("Game Settings")]
    public class HoldableDatabase : ScriptableSettings
    {
        [SerializeField] private SerializedDictionary<ItemSO, SerializableInterface<IHoldable>> _holdables;

        public IReadOnlyDictionary<ItemSO, SerializableInterface<IHoldable>> Holdables => _holdables;
    }
}