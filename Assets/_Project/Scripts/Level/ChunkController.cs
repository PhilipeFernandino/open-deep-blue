using Coimbra;
using Core.EventBus;
using Core.Map;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Tilemaps;
using static Core.Util.Range;

namespace Core.Level
{
    public class ChunkController : MonoBehaviour
    {
        [SerializeField] private TileBase _debugTileBase;

        [SerializeField] private PositionEventBus _positionEventBus;
        [SerializeField] private int _chunkSize = 16;
        [SerializeField] private int _loadNearChunks = 1;

        private TilesSettings _tilesSettings;

        private HashSet<Vector2Int> _tilemapOn = new();
        private HashSet<Vector2Int> _chunkAnchors = new();

        private Tilemap _tilemap;
        private Tilemap _floorTilemap;

        private List<Vector2Int> _chunksToRemove = new();

        private MapMetadata _mapMetadata;
        TileBase[] _emptyTiles;

        private void Awake()
        {
            _positionEventBus.PositionChanged += PositionChanged_EventHandler;
            _tilesSettings = ScriptableSettings.GetOrFind<TilesSettings>();
        }

        public void Setup(MapMetadata mapMetadata, Tilemap tilemap, Tilemap floorTilemap)
        {
            _mapMetadata = mapMetadata;
            _tilemap = tilemap;
            _floorTilemap = floorTilemap;
            _emptyTiles = new TileBase[_chunkSize * _chunkSize];

            PositionChanged_EventHandler(_positionEventBus.Position);
        }

        private void PositionChanged_EventHandler(Vector2 vector)
        {
            Vector2Int pos = Vector2Int.RoundToInt(vector);

            if (_mapMetadata == null)
            {
                return;
            }

            Vector2Int currChunk = new(
                ToClosestLowerMultiple(pos.x, _chunkSize),
                ToClosestLowerMultiple(pos.y, _chunkSize));

            Debug.Log($"{GetType()} - (Pos = {vector}, PosChunk = {currChunk})");

            _chunkAnchors.Clear();

            for (int i = -_loadNearChunks; i <= _loadNearChunks; i++)
            {
                for (int j = -_loadNearChunks; j <= _loadNearChunks; j++)
                {
                    var chunkAnchor = currChunk + new Vector2Int(i, j) * _chunkSize;
                    _chunkAnchors.Add(chunkAnchor);
                }
            }

            foreach (var chunkAnchor in _chunkAnchors)
            {
                if (!_tilemapOn.Contains(chunkAnchor))
                {
                    SetTileChunk(chunkAnchor);
                }
            }

            _chunksToRemove.Clear();

            foreach (var tilemapOn in _tilemapOn)
            {
                if (!_chunkAnchors.Contains(tilemapOn))
                {
                    _chunksToRemove.Add(tilemapOn);
                }
            }

            // Unload and remove old chunks
            foreach (var toRm in _chunksToRemove)
            {
                UnsetTileChunk(toRm);
                _tilemapOn.Remove(toRm);
            }
        }

        private void SetTileChunk(Vector2Int chunkAnchor)
        {
            BoundsInt area = new();
            area.SetMinMax(
                new(chunkAnchor.x, chunkAnchor.y, 0),
                new(chunkAnchor.x + _chunkSize, chunkAnchor.y + _chunkSize, 1));

            TileBase[] tiles = new TileBase[_chunkSize * _chunkSize];
            TileBase[] floorTiles = new TileBase[_chunkSize * _chunkSize];
            Debug.Log($"{GetType()} - {area}");

            for (int w = chunkAnchor.y; w < chunkAnchor.y + _chunkSize; w++)
            {
                for (int h = chunkAnchor.x; h < chunkAnchor.x + _chunkSize; h++)
                {
                    if (IsWithinBounds(w, h, 0, 0, _mapMetadata.Dimensions, _mapMetadata.Dimensions))
                    {
                        int ww = w - chunkAnchor.y;
                        int hh = h - chunkAnchor.x;

                        int index = ww * _chunkSize + hh;

                        tiles[index] = _tilesSettings.GetTileBase(_mapMetadata.Tiles[h, w]);
                        floorTiles[index] = _tilesSettings.GetFloorTileBase(_mapMetadata.BiomeTiles[h, w]);
                    }
                }
            }

            _tilemap.SetTilesBlock(area, tiles);
            _floorTilemap.SetTilesBlock(area, floorTiles);

            Debug.Log($"{GetType()} - Tilemap added for: {chunkAnchor}");

            _tilemapOn.Add(chunkAnchor);
        }

        private void UnsetTileChunk(Vector2Int chunkAnchor)
        {
            BoundsInt area = new();
            area.SetMinMax(
                new(chunkAnchor.x, chunkAnchor.y, 0),
                new(chunkAnchor.x + _chunkSize, chunkAnchor.y + _chunkSize, 1));

            _tilemap.SetTilesBlock(area, _emptyTiles);
            _floorTilemap.SetTilesBlock(area, _emptyTiles);
        }
    }
}