using Coimbra;
using Coimbra.Services;
using Coimbra.Services.Events;
using Core.Colony;
using Core.Units;
using System.Collections.Generic;
using Unity.MLAgents;
using UnityEngine;
using UnityEngine.Pool;

namespace Core.Train
{
    public class ColonyManager : Actor, IColonyService
    {
        [Header("Ant Spawning")]
        [SerializeField] private Ant _antPrefab;
        [SerializeField] private int _initialAntCount;
        [SerializeField] private int _poolCapacity = 50;

        [Header("Dependencies")]
        [SerializeField] private CurriculumManager _curriculumManager;

        private SimpleMultiAgentGroup _agentGroup = new();
        private HashSet<AntAgent> _ants = new();

        private IObjectPool<Ant> _antPool;

        public void AddGroupReward(float value) => _agentGroup.AddGroupReward(value);
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

        protected override void OnInitialize()
        {
            ServiceLocator.Set<IColonyService>(this);

            _antPool = new ObjectPool<Ant>(
                createFunc: () => Instantiate(_antPrefab),
                actionOnGet: (ant) => ant.gameObject.SetActive(true),
                actionOnRelease: (ant) => ant.gameObject.SetActive(false),
                actionOnDestroy: (ant) => ant.gameObject.Dispose(true),
                collectionCheck: false,
                defaultCapacity: _initialAntCount,
                maxSize: _poolCapacity
            );

            AntEvent.AddListener(AntEventHandler);
            ColonyEvent.AddListener(ColonyEventHandler);
            _curriculumManager.EnvironmentSetup += EnvironmentSetupEventHandler;
        }

        public void SpawnAnt()
        {
            Ant ant = _antPool.Get();
            ant.Initialize();
        }

        public void DespawnAnt(Ant ant)
        {
            _antPool.Release(ant);
        }

        private void EnvironmentSetupEventHandler()
        {
            for (int i = 0; i < _initialAntCount; i++)
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
                    DespawnAnt(e.Ant);
                    break;
            }
        }

        private void ColonyEventHandler(ref EventContext context, in ColonyEvent e)
        {
            switch (e.EventType)
            {
                case ColonyEventType.QueenProcreation:
                    SpawnAnt();
                    break;
            }
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