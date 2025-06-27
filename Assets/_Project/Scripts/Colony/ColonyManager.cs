using Coimbra;
using Coimbra.Services;
using Coimbra.Services.Events;
using Core.Colony;
using Core.Units;
using System.Collections.Generic;
using Unity.MLAgents;
using UnityEngine;

namespace Core.Train
{

    public class ColonyManager : Actor, IColonyService
    {
        [SerializeField] private Ant _antPrefab;
        [SerializeField] private int _spawnAnts;
        [SerializeField] private CurriculumManager _curriculumManager;

        private SimpleMultiAgentGroup _agentGroup = new();
        private HashSet<AntAgent> _ants = new();

        public void SpawnAnt()
        {
            var ant = Instantiate(_antPrefab);
            ant.Initialize();
        }

        protected override void OnInitialize()
        {
            ServiceLocator.Set<IColonyService>(this);
            AntEvent.AddListener(AntEventHandler);
            _curriculumManager.EnvironmentSetup += EnvironmentSetupEventHandler;
        }

        private void EnvironmentSetupEventHandler()
        {
            for (int i = 0; i < _spawnAnts; i++)
            {
                SpawnAnt();
            }
        }

        private void AntEventHandler(ref EventContext context, in AntEvent e)
        {
            switch (e.AntEventType)
            {
                case AntEventType.Setup:
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

    [DynamicService]
    public interface IColonyService : IService
    {
        public void AddGroupReward(float value);
        public void RegisterAnt(AntAgent agent);
        public void UnregisterAnt(AntAgent agent);
    }
}