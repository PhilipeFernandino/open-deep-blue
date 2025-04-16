using Core.EventBus;
using Core.Light;
using UnityEngine;

namespace Core.HoldableSystem
{
    public class Torch : Equipment
    {
        [SerializeField] private ObjectAddedEventBus _addLightEventBus;

        public LightSourceAttributesSO Attributes => (LightSourceAttributesSO)_attributes;

        protected override void UseBehavior(Vector2 position)
        {
            _addLightEventBus.AddObject(new LightSource(Vector2Int.RoundToInt(position), Attributes.Intensity));
        }
    }
}
