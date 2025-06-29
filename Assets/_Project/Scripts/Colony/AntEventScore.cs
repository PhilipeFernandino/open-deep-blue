using System;

namespace Core.Train
{
    [Serializable]
    public struct AntEventScore
    {
        public AntEventType EventType;
        public Reward Reward;
    }
}