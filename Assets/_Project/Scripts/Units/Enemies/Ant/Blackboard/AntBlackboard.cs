using Core.Level;
using UnityEngine;

namespace Core.Units
{
    public class AntBlackboard : MonoBehaviour
    {
        public Blackboard<AntPheromone> PheromoneLevel;
        public Blackboard<AntBlackboardKeys> Internal;

        public int GetPheromoneLevel(AntPheromone key)
        {
            if (PheromoneLevel.TryGet(key, out int value))
            {
                return value;
            }
            return default;
        }

        public void SetPheromoneLevel(AntPheromone key, int value)
        {
            PheromoneLevel.Set(key, value);
        }

        public T GetInternal<T>(AntBlackboardKeys key)
        {
            if (Internal.TryGet(key, out object value))
            {
                return (T)value;
            }
            return default;
        }

        public void SetInternal<T>(AntBlackboardKeys key, T value)
        {
            Internal.Set(key, value);
        }
    }
}