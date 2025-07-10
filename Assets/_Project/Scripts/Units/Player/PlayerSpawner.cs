using Core.Level;
using Core.Save;
using Core.Scene;
using Core.Util;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Core.Player
{
    public class PlayerSpawner : MonoBehaviour
    {
        [SerializeField] private Player _playerPrefab;
        [SerializeField] private Transform _defaultPosition;

        private IGridService _gridService;

        private void Start()
        {
            _gridService = ServiceLocatorUtilities.GetServiceAssert<IGridService>();
            _gridService.Initialized += GridInitialized;
        }

        private void GridInitialized()
        {
            Vector2 position = _defaultPosition.position;

            switch ((GameScene)SceneManager.GetActiveScene().buildIndex)
            {
                case GameScene.BurrowHole:
                    IGridService gridService = ServiceLocatorUtilities.GetServiceAssert<IGridService>();
                    var (found, stairPos) = gridService.MapMetadata.FirstTile(Map.Tile.Stair);
                    if (found)
                    {
                        position = stairPos + Vector2.down;
                    }
                    break;

                case GameScene.Cave:
                    if (GameState.SessionData.HasOverworldData)
                    {
                        GameState.SessionData.HasOverworldData = false;
                        position = GameState.SessionData.PlayerReturnPosition;
                    }
                    break;
            }

            SpawnPlayer(position);
        }

        private void SpawnPlayer(Vector2 spawnPosition)
        {
            var player = Instantiate(_playerPrefab, spawnPosition, Quaternion.identity);
            player.Initialize();
        }
    }

}