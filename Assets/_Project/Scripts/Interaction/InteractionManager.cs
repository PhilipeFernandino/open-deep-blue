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

    public class InteractionManager : Actor, IInteractionService
    {
        [SerializeField] private TileInteractionMappingSO _tileInteractionMap;

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

        public bool TryInteract(Vector2 worldPosition, MonoBehaviour interactor)
        {
            if (CanInteract(worldPosition, out TileInstance tile))
            {
                ProcessInteraction(tile.TileType, worldPosition, interactor);
                return true;
            }

            return false;
        }

        private void ProcessInteraction(Tile tile, Vector2 worldPosition, MonoBehaviour interactor)
        {
            Debug.Log($"Interacting with {tile} at {worldPosition}");

            if (_tileInteractionMap.TryGetValue(tile, out var effect))
            {
                effect.Execute(interactor, worldPosition);

            }
        }

        protected override void OnInitialize()
        {
            ServiceLocator.Set<IInteractionService>(this);

            OnStarting += InteractionManager_OnStarting;
        }

        private void InteractionManager_OnStarting(Actor sender)
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
        public bool TryInteract(Vector2 worldPosition, MonoBehaviour interactor);

    }
}
