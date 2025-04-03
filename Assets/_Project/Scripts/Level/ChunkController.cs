using Core.EventBus;
using Core.Map;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Core.Level
{
    public class ChunkController : MonoBehaviour
    {
        [SerializeField] private PositionEventBus _positionEventBus;
        [SerializeField] private int _chunkSize = 16;
        [SerializeField] private int _loadNearChunks = 1;

        private TilesSettings _tilesSettings;

        private Tilemap[] _tilemaps;

        private Dictionary<Vector2Int, Tilemap> _tilemapOn;


        private Map.Map _map;

        private void Awake()
        {
            _positionEventBus.PositionChanged += PositionChanged_EventHandler;
        }

        public void Setup(Map.Map map)
        {
            _map = map;
        }

        private void PositionChanged_EventHandler(Vector2 vector)
        {
            Debug.Log($"{GetType()} - position changed to {vector}");

            int dim = _map.Metadata.Dimensions;
            Vector2 playerPos = _positionEventBus.Position;
            Vector2Int playerChunk = new(
                ToClosestLowerMultiple((int)playerPos.x, _chunkSize),
                ToClosestLowerMultiple((int)playerPos.x, _chunkSize));

            for (int i = -_loadNearChunks; i <= _loadNearChunks; i++)
            {
                for (int j = -_loadNearChunks; j <= _loadNearChunks; j++)
                {
                    var chunkPos = playerChunk + new Vector2Int(i, j);
                    if (!_tilemapOn.TryGetValue(chunkPos, out var _))
                    {
                        int mapDim = _map.Metadata.Dimensions;
                        var tilemap = Instantiate(new GameObject(), Vector3.zero, Quaternion.identity).AddComponent<Tilemap>();
                        _tilemapOn.Add(chunkPos, tilemap);

                        BoundsInt area = new();
                        area.SetMinMax(
                            new(chunkPos.x, chunkPos.y, 1),
                            new(chunkPos.x + _chunkSize, chunkPos.y + _chunkSize, 1));

                        //TileBase[] tiles = new TileBase[dimensions * dimensions];
                        //TileBase[] floorTiles = new TileBase[];

                        //for (int w = chunkPos.x; w < chunkPos.x + _chunkSize; w++)
                        //{
                        //    for (int h = chunkPos.y; h < chunkPos.y + _chunkSize; h++)
                        //    {
                        //        int index = w * _chunkSize + h;
                        //        tiles[index] = _tilesSettings.GetTileBase(_map.Metadata.Tiles[h, w]);
                        //        floorTiles[index] = _tilesSettings.GetFloorTileBase(_map.Metadata.BiomeTiles[h, w]);
                        //    }
                        //}

                        tilemap.SetTilesBlock(area, _map.TileBases[_..1]);

                    }
                }
            }
        }

        private int ToClosestLowerMultiple(int value, int multiplier)
        {
            return value - value % multiplier;
        }
    }
}