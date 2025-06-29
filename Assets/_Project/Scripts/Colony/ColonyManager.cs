using Coimbra;
using Coimbra.Services;
using Coimbra.Services.Events;
using Core.Colony;
using Core.Units;
using Cysharp.Threading.Tasks;
using System;
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
        [SerializeField] private int _poolCapacity = 25;

        [Header("Dependencies")]
        [SerializeField] private CurriculumManager _curriculumManager;

        private SimpleMultiAgentGroup _agentGroup = new();
        private HashSet<AntAgent> _ants = new();

        private IObjectPool<Ant> _antPool;

        public void EndGroupEpisode() => _agentGroup.EndGroupEpisode();
        public void AddGroupReward(float value) => _agentGroup.AddGroupReward(value);

        private void RegisterAnt(AntAgent agent)
        {
            _agentGroup.RegisterAgent(agent);
            _ants.Add(agent);
            Debug.Log($"Register ant: {agent.gameObject.name}", this);
        }

        private void UnregisterAnt(AntAgent agent)
        {
            _agentGroup.UnregisterAgent(agent);
            _ants.Remove(agent);
            Debug.Log($"Unregister ant: {agent.gameObject.name}", this);
        }

        protected override void OnInitialize()
        {
            ServiceLocator.Set<IColonyService>(this);

            _antPool = new ObjectPool<Ant>(
                createFunc: () =>
                {
                    var ant = Instantiate(_antPrefab);
                    ant.Initialize();
                    ant.Agent.SpawnPointRequested += AgentSpawnPointRequested;
                    return ant;
                },
                actionOnGet: (ant) =>
                {
                    ant.gameObject.SetActive(true);
                    ant.gameObject.name = $"Ant {ant.gameObject.GetInstanceID()}";

                    ant.ResetState();

                    RegisterAnt(ant.Agent);
                },
                actionOnRelease: (ant) =>
                {
                    ant.gameObject.SetActive(false);
                    ant.Agent.EndEpisode();
                    UnregisterAnt(ant.Agent);
                },
                actionOnDestroy: (ant) =>
                {
                    ant.gameObject.Dispose(true);
                    ant.Agent.SpawnPointRequested -= AgentSpawnPointRequested;
                },
                defaultCapacity: _initialAntCount,
                maxSize: _poolCapacity
            );

            AntEvent.AddListener(AntEventHandler);
            ColonyEvent.AddListener(ColonyEventHandler);
            _curriculumManager.EnvironmentSetup += EnvironmentSetupEventHandler;
        }

        private Vector2 AgentSpawnPointRequested()
        {
            var pos = _curriculumManager.CurrentLessonHandler.GetSpawnPoint();
            return pos;
        }

        public void SpawnAnt()
        {
            _antPool.Get();
        }

        public void DespawnAnt(Ant ant)
        {
            _antPool.Release(ant);
        }

        public void EndAgentEpisode(Agent agent)
        {
            agent.EndEpisode();
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
                case AntEventType.Death:
                    HandleDeath(e).Forget();
                    break;
            }
        }

        private async UniTask HandleDeath(AntEvent e)
        {
            await UniTask.DelayFrame(1);
            DespawnAnt(e.Ant);
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
        public void EndGroupEpisode();
        public void AddGroupReward(float value);
        public void EndAgentEpisode(Agent agent);
    }
}