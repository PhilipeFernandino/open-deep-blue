using Coimbra.Services.Events;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Level
{
    public readonly partial struct ChunksUpdated : IEvent
    {
        public readonly List<Vector2Int> LoadedChunks;
        public readonly List<Vector2Int> UnloadedChunks;
        public readonly int ChunkSize;
        public readonly Vector2Int Origin;

        public ChunksUpdated(List<Vector2Int> loadedChunks, List<Vector2Int> unloadedChunks, int chunkSize, Vector2Int origin)
        {
            LoadedChunks = loadedChunks;
            UnloadedChunks = unloadedChunks;
            ChunkSize = chunkSize;
            Origin = origin;
        }

        public static void AddListener(object chunksUpdated_EventHandler)
        {
            throw new NotImplementedException();
        }
    }
}