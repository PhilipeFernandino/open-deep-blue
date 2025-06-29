using System;

namespace Core.Train
{
    [Serializable]
    public struct Reward
    {
        public float AgentReward;
        public float GroupReward;

        public override string ToString()
        {
            return $"(AgentReward = {AgentReward}, GroupReward = {GroupReward})";
        }
    }
}