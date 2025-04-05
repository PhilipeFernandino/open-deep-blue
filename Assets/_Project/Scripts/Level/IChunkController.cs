using System.Collections;
using UnityEngine;

namespace Core.Level
{
    public interface IChunkController
    {
        public void SetTileChunk(BoundsInt area, Vector2Int anchor);
        public void UnsetTileChunk(BoundsInt area, Vector2Int anchor);
        public void SetOrigin(Vector2Int origin);
    }
}