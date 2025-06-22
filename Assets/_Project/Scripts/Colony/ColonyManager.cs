using Coimbra;
using Coimbra.Services;
using Coimbra.Services.Events;
using Core.Colony;
using Core.Units;
using Core.Util;
using System;
using System.Collections.Generic;
using System.Numerics;
using Unity.MLAgents;

namespace Core.Train
{

    public class ColonyManager : Actor, IColonyService
    {
        private SimpleMultiAgentGroup _agentGroup = new();
        private HashSet<AntAgent> _ants = new();

        protected override void OnInitialize()
        {
            ServiceLocator.Set<IColonyService>(this);
            AntEvent.AddListener(AntEventHandler);
        }

        private void AntEventHandler(ref EventContext context, in AntEvent e)
        {
            switch (e.AntEventType)
            {
                case AntEventType.Born:
                    RegisterAnt(e.Ant.Agent);
                    break;
                case AntEventType.Death:
                    UnregisterAnt(e.Ant.Agent);
                    break;
            }
        }

        public void AddGroupReward(float value)
        {
            _agentGroup.AddGroupReward(value);
        }

        public void RegisterAnt(AntAgent agent)
        {
            _agentGroup.RegisterAgent(agent);
            _ants.Add(agent);
        }

        public void UnregisterAnt(AntAgent agent)
        {
            _agentGroup.UnregisterAgent(agent);
            _ants.Remove(agent);
        }
    }
}