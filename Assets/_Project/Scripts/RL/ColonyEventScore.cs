using System;

namespace Core.RL
{
    [Serializable]
    public struct ColonyEventScore
    {
        public ColonyEventType EventType;
        public Reward Reward;
    }
}