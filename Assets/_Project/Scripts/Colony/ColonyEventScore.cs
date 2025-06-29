using System;

namespace Core.Train
{
    [Serializable]
    public struct ColonyEventScore
    {
        public ColonyEventType EventType;
        public Reward Reward;
    }
}