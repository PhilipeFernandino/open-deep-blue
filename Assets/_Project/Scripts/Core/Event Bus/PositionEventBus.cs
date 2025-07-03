using System;
using UnityEngine;

namespace Core.EventBus
{
    [CreateAssetMenu(menuName = "Core/EventBus/ObjectAddedEventBus")]
    public class PositionEventBus : ScriptableObject
    {
        private Vector2 _position = Vector2.zero;

        public Vector2 Position
        {
            get => _position; set
            {
                if (_position != value)
                {
                    _position = value;
                    PositionChanged?.Invoke(_position);
                }
            }
        }

        public Action<Vector2> PositionChanged;

        public void Trigger()
        {
            PositionChanged?.Invoke(_position);
        }
    }

}