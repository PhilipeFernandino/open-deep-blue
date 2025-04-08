using Core.Util;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Level
{
    [System.Serializable]
    public class ChunkController
    {
        public int ChunkSize => _chunkSize;
        public int LoadNearChunks => _loadNearChunks;
        public int LoadedDimensions => _chunkSize * (_loadNearChunks * 2 + 1);
        public HashSet<Vector2Int> ActiveChunks => _loadedChunkAnchors;

        private int _chunkSize;
        private int _loadNearChunks;
        private bool _useLookahead;

        private HashSet<Vector2Int> _loadedChunkAnchors = new();

        private HashSet<Vector2Int> _updatedChunkAnchors = new();
        private List<Vector2Int> _chunkAnchorsToRemove = new();
        private List<Vector2Int> _chunkAnchorsToAdd = new();

        public event Action<Vector2Int> OriginSetted;
        public event Action<(BoundsInt area, Vector2Int anchor)> TileChunkSetted;
        public event Action<(BoundsInt area, Vector2Int anchor)> TileChunkUnsetted;
        public event Action<HashSet<Vector2Int>> TileChunksUpdated;

        private Vector2Int _previousChunk;

        public ChunkController(int chunkSize, int loadNearChunks, bool useLookahead = false)
        {
            _chunkSize = chunkSize;
            _loadNearChunks = loadNearChunks;
            _useLookahead = useLookahead;
        }


        public Vector2Int ToChunk(Vector2Int pos)
        {
            return new(
                Util.Range.ToClosestLowerMultiple(pos.x, ChunkSize),
                Util.Range.ToClosestLowerMultiple(pos.y, ChunkSize));
        }

        public void UpdatePosition(Vector2 vector)
        {
            _chunkAnchorsToRemove.Clear();
            _chunkAnchorsToAdd.Clear();
            _updatedChunkAnchors.Clear();

            Vector2Int pos = Vector2Int.RoundToInt(vector);

            Vector2Int currChunk = ToChunk(pos);

            if (_previousChunk == currChunk)
            {
                return;
            }

            for (int i = -_loadNearChunks; i <= _loadNearChunks; i++)
            {
                for (int j = -_loadNearChunks; j <= _loadNearChunks; j++)
                {
                    var chunkAnchor = currChunk + new Vector2Int(i, j) * _chunkSize;
                    _updatedChunkAnchors.Add(chunkAnchor);
                }
            }

            OriginSetted?.Invoke(currChunk + new Vector2Int(-_loadNearChunks, -_loadNearChunks) * _chunkSize);

            // Prepare to add and to remove lists
            foreach (var chunkAnchor in _updatedChunkAnchors)
            {
                if (!_loadedChunkAnchors.Contains(chunkAnchor))
                {
                    _chunkAnchorsToAdd.Add(chunkAnchor);
                }
            }

            foreach (var loadedChunk in _loadedChunkAnchors)
            {
                if (!_updatedChunkAnchors.Contains(loadedChunk))
                {
                    if (Vector2.Distance(loadedChunk, currChunk) >= _chunkSize * 3 || !_useLookahead)
                    {
                        _chunkAnchorsToRemove.Add(loadedChunk);
                    }
                }
            }

            // Unload an load new chunks
            foreach (var anchorTorRemove in _chunkAnchorsToRemove)
            {
                BoundsInt area = new();
                area.SetMinMax(
                    new(anchorTorRemove.x, anchorTorRemove.y, 0),
                    new(anchorTorRemove.x + ChunkSize, anchorTorRemove.y + ChunkSize, 1));

                TileChunkUnsetted?.Invoke((area, anchorTorRemove));
                _loadedChunkAnchors.Remove(anchorTorRemove);
            }

            foreach (var anchorToAdd in _chunkAnchorsToAdd)
            {
                BoundsInt area = new();
                area.SetMinMax(
                    new(anchorToAdd.x, anchorToAdd.y, 0),
                    new(anchorToAdd.x + ChunkSize, anchorToAdd.y + ChunkSize, 1));

                TileChunkSetted?.Invoke((area, anchorToAdd));
                _loadedChunkAnchors.Add(anchorToAdd);
            }

            _previousChunk = currChunk;
            TileChunksUpdated?.Invoke(ActiveChunks);
        }
    }
}