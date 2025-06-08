using Coimbra;
using Core.EventBus;
using Core.Map;
using UnityEngine;
using UnityEngine.Tilemaps;
using static Core.Util.Range;

namespace Core.Level
{
    [System.Serializable]
    public class TileChunkController
    {
        private PositionEventBus _positionEventBus;

        private ChunkController _chunkController;

        private int ChunkSize => _chunkController.ChunkSize;
        private int LoadNearChunks => _chunkController.LoadNearChunks;

        private IGridService _gridService;
        private TilesSettings _tilesSettings;
        private MapMetadata _mapMetadata;
        TileBase[] _emptyTiles;

        public TileChunkController(MapMetadata mapMetadata, int chunkSize, int loadNearChunks,
            PositionEventBus positionEventBus, IGridService gridService, TilesSettings tilesSettings)
        {
            _chunkController = new(chunkSize, loadNearChunks);

            _chunkController.TileChunkSetted += SetTileChunk;
            _chunkController.TileChunkUnsetted += UnsetTileChunk;

            _mapMetadata = mapMetadata;
            _emptyTiles = new TileBase[chunkSize * chunkSize];
            _positionEventBus = positionEventBus;
            _gridService = gridService;
            _tilesSettings = tilesSettings;

            _positionEventBus.PositionChanged += PositionChanged_EventHandler;
            PositionChanged_EventHandler(_positionEventBus.Position);
        }

        public void UpdatePosition(Vector2 vector)
        {
            if (_mapMetadata == null)
            {
                Debug.LogWarning($"{GetType()} - {nameof(UpdatePosition)} Map metadata is null");
                return;
            }

            _chunkController.UpdatePosition(vector);
        }

        private void PositionChanged_EventHandler(Vector2 vector)
        {
            UpdatePosition(vector);
        }

        public void SetTileChunk((BoundsInt area, Vector2Int anchor) e)
        {
            BoundsInt area = e.area;
            Vector2Int anchor = e.anchor;

            TileBase[] tiles = new TileBase[ChunkSize * ChunkSize];
            TileBase[] floorTiles = new TileBase[ChunkSize * ChunkSize];

            Debug.Log($"{GetType()} - {area}");

            for (int w = anchor.y; w < anchor.y + ChunkSize; w++)
            {
                for (int h = anchor.x; h < anchor.x + ChunkSize; h++)
                {
                    if (IsWithinBounds(w, h, 0, _mapMetadata.Dimensions))
                    {
                        int ww = w - anchor.y;
                        int hh = h - anchor.x;

                        int index = ww * ChunkSize + hh;

                        // get from grid tiles instead of mapmetad
                        tiles[index] = _tilesSettings.GetTileBase(_mapMetadata.Tiles[h, w]);
                        floorTiles[index] = _tilesSettings.GetFloorTileBase(_mapMetadata.BiomeTiles[h, w]);
                    }
                }
            }

            _gridService.LoadTilesBlock(area, tiles);
            _gridService.LoadFloorTilesBlock(area, floorTiles);

            Debug.Log($"{GetType()} - Tilemap added for: {anchor}");
        }

        public void UnsetTileChunk((BoundsInt area, Vector2Int anchor) e)
        {
            _gridService.LoadTilesBlock(e.area, _emptyTiles);
            _gridService.LoadFloorTilesBlock(e.area, _emptyTiles);
        }
    }
}