using Coimbra.Services;
using Google.Protobuf.WellKnownTypes;
using System;
using UnityEngine;

namespace Core.Level
{
    [DynamicService]
    public interface IChemicalGridService : IService
    {
        public event Action Initialized;

        public int Dimensions { get; }

        public ChemicalMap GetMap(Chemical chemical);

        public bool TryGetMap(Chemical chemical, out ChemicalMap value);

        public float Get(int x, int y, Chemical chemical);

        public void Drop(int x, int y, Chemical chemical, float value);

        public void Remove(int x, int y, Chemical chemical, float value);

        public void Clean(int x, int y, Chemical chemical);

        public float Get(Vector2 v, Chemical chemical)
        {
            var (x, y) = ToGridPosition(v);
            return Get(x, y, chemical);
        }

        public void Drop(Vector2 v, Chemical chemical, float value)
        {
            var (x, y) = ToGridPosition(v);
            Drop(x, y, chemical, value);
        }

        public void Remove(Vector2 v, Chemical chemical, float value)
        {
            var (x, y) = ToGridPosition(v);
            Remove(x, y, chemical, value);
        }

        public void Clean(Vector2 v, Chemical chemical)
        {
            var (x, y) = ToGridPosition(v);
            Clean(x, y, chemical);
        }

        public static (int x, int y) ToGridPosition(Vector2 v)
        {
            Vector2Int v2 = new(Mathf.RoundToInt(v.x), Mathf.RoundToInt(v.y));
            return (v2.x, v2.y);
        }
    }
}