using UnityEngine;

namespace Core.Map
{
    public struct PointOfInterest
    {
        public int X;
        public int Y;
        public PointOfInterestType Type;

        public readonly Vector2Int Position => new Vector2Int(X, Y);

        public PointOfInterest(int x, int y, PointOfInterestType type)
        {
            X = x;
            Y = y;
            Type = type;
        }

        public override string ToString()
        {
            return $"X: {X}, Y: {Y}, Type: {Type}";
        }
    }
}