using Coimbra;
using Core.EventBus;
using Core.Level;
using Core.Map;
using Core.Util;
using UnityEngine;

namespace Core.Interaction
{
    public class InteractionPlayer : MonoBehaviour
    {
        [SerializeField] private PositionEventBus _positionEventBus;

        private Player.Player _player;

        private Vector2Int _lastPosition;
        private IGridService _gridService;
        private TilesSettings _tileSettings;

        private TileInstance _interactableTile;

        public bool TryInteract()
        {
            if (_interactableTile == null)
            {
                return false;
            }


            return true;
        }

        private void Start()
        {
            _gridService = ServiceLocatorUtilities.GetServiceAssert<IGridService>();
            _tileSettings = ScriptableSettings.GetOrFind<TilesSettings>();

            _positionEventBus.PositionChanged += PositionChangedEventHandler;
        }

        private void PositionChangedEventHandler(Vector2 vector)
        {
            if (_player == null)
            {
                _player = FindAnyObjectByType<Player.Player>();
            }

            Vector2Int v2 = (vector + _player.FacingDirection).RoundToInt();

            if (v2 != _lastPosition)
            {
                return;
            }

            _lastPosition = v2;

            if (_gridService.TryGetTileAt(v2, out TileInstance tile))
            {
                var tileProperties = _tileSettings.GetDefinition(tile.TileType);

                if (tileProperties.TileProperties.HasFlag(TileProperties.IsInteractable))
                {
                    _interactableTile = tile;
                }
                else
                {
                    _interactableTile = TileInstance.None;
                }
            }
            else
            {
                _interactableTile = TileInstance.None;
            }
        }
    }
}