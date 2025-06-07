using Coimbra;
using Coimbra.Services;
using Core.Level;
using Core.Map;
using Core.Util;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Interaction
{
    [Serializable]
    public class TileInteractionEffect { public Tile Tile; public List<InteractionEffectSO> InteractionEffects; }


    public class InteractionManager : Actor, IInteractionService
    {
        [SerializeField] private List<TileInteractionEffect> _tileInteractionEffects;

        private Dictionary<Tile, List<InteractionEffectSO>> _effectsDict;

        private IGridService _grid;
        private TilesSettings _tilesSettings;


        public bool CanInteract(Vector2 worldPosition)
        {
            return _grid.TryGetTileAt(worldPosition, out TileInstance tile) && _tilesSettings.GetDefinition(tile.TileType).TileProperties.HasFlag(TileProperties.IsInteractable);
        }

        public bool CanInteract(Vector2 worldPosition, out TileInstance tile)
        {
            return _grid.TryGetTileAt(worldPosition, out tile) && _tilesSettings.GetDefinition(tile.TileType).TileProperties.HasFlag(TileProperties.IsInteractable);
        }

        public void Interact(Vector2 worldPosition, MonoBehaviour interactor)
        {
            if (CanInteract(worldPosition, out TileInstance tile))
            {
                ProcessInteraction(tile.TileType, worldPosition, interactor);
            }
        }

        private void ProcessInteraction(Tile tile, Vector2 worldPosition, MonoBehaviour interactor)
        {
            Debug.Log($"Interacting with {tile} at {worldPosition}");

            if (_effectsDict.TryGetValue(tile, out var effects))
            {
                foreach (var effect in effects)
                {
                    effect.Execute(interactor, worldPosition);
                }

            }
        }

        protected override void OnInitialize()
        {
            _effectsDict = new();
            foreach (var effect in _tileInteractionEffects)
            {
                _effectsDict.Add(effect.Tile, effect.InteractionEffects);
            }

            ServiceLocator.Set<IInteractionService>(this);
        }

        protected override void OnSpawn()
        {
            _grid = ServiceLocatorUtilities.GetServiceAssert<IGridService>();
            _tilesSettings = ScriptableSettings.GetOrFind<TilesSettings>();
        }
    }

    [DynamicService]
    public interface IInteractionService : IService
    {
        public bool CanInteract(Vector2 worldPosition);
        public bool CanInteract(Vector2 worldPosition, out TileInstance tile);
        public void Interact(Vector2 worldPosition, MonoBehaviour interactor);

    }
}
