using System;

namespace Core.RL
{
    [Serializable]
    public struct AntEventScore
    {
        public AntEventType EventType;
        public Reward Reward;
    }
}