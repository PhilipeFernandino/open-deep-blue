using UnityEngine;

namespace Core.Map
{
    public struct PointOfInterest
    {
        public int X;
        public int Y;
        public PointOfInterestType Type;

        public readonly Vector2Int Position => new Vector2Int(X, Y);
    }
}