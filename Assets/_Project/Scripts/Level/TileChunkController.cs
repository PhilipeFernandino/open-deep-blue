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

        private TilesSettings _tilesSettings;

        private Tilemap _tilemap;
        private Tilemap _floorTilemap;

        private MapMetadata _mapMetadata;
        TileBase[] _emptyTiles;

        public TileChunkController(MapMetadata mapMetadata, Tilemap tilemap, Tilemap floorTilemap, int chunkSize, int loadNearChunks,
            PositionEventBus positionEventBus, TilesSettings tilesSettings)
        {
            _chunkController = new(chunkSize, loadNearChunks);

            _chunkController.TileChunkSetted += SetTileChunk;
            _chunkController.TileChunkUnsetted += UnsetTileChunk;

            _mapMetadata = mapMetadata;
            _tilemap = tilemap;
            _floorTilemap = floorTilemap;
            _emptyTiles = new TileBase[chunkSize * chunkSize];
            _tilesSettings = tilesSettings;
            _positionEventBus = positionEventBus;

            _positionEventBus.PositionChanged += PositionChanged_EventHandler;
            PositionChanged_EventHandler(_positionEventBus.Position);
        }

        private void PositionChanged_EventHandler(Vector2 vector)
        {
            if (_mapMetadata == null)
            {
                return;
            }

            _chunkController.UpdatePosition(vector);
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
                    if (IsWithinBounds(w, h, 0, 0, _mapMetadata.Dimensions, _mapMetadata.Dimensions))
                    {
                        int ww = w - anchor.y;
                        int hh = h - anchor.x;

                        int index = ww * ChunkSize + hh;

                        tiles[index] = _tilesSettings.GetTileBase(_mapMetadata.Tiles[h, w]);
                        floorTiles[index] = _tilesSettings.GetFloorTileBase(_mapMetadata.BiomeTiles[h, w]);
                    }
                }
            }

            _tilemap.SetTilesBlock(area, tiles);
            _floorTilemap.SetTilesBlock(area, floorTiles);

            Debug.Log($"{GetType()} - Tilemap added for: {anchor}");
        }

        public void UnsetTileChunk((BoundsInt area, Vector2Int anchor) e)
        {
            _tilemap.SetTilesBlock(e.area, _emptyTiles);
            _floorTilemap.SetTilesBlock(e.area, _emptyTiles);
        }
    }
}