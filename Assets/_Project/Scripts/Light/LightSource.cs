using UnityEngine;

namespace Core.Light
{
    [System.Serializable]
    public readonly struct LightSource
    {
        public readonly Vector2Int Position;
        public readonly uint Intensity;
        public readonly Color Color;

        public LightSource(Vector2Int pos, uint intensity, Color color = default)
        {
            Position = pos;
            Intensity = intensity;
            Color = color == default ? Color.white : color;
        }

    }
}
