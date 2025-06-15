using Coimbra;
using Coimbra.Services;
using Core.Units;
using System.Collections.Generic;
using Unity.MLAgents;

namespace Core.Train
{

    public class ColonyManager : Actor, IColonyService
    {
        private SimpleMultiAgentGroup _agentGroup = new();

        protected override void OnInitialize()
        {
            ServiceLocator.Set<IColonyService>(this);
        }

        public void AddGroupReward(float value)
        {
            _agentGroup.AddGroupReward(value);
        }

        public void RegisterAnt(AntAgent agent)
        {
            _agentGroup.RegisterAgent(agent);
        }

        public void UnregisterAnt(AntAgent agent)
        {
            _agentGroup.UnregisterAgent(agent);
        }
    }
}