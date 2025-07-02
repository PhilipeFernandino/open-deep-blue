using Coimbra;
using Coimbra.Services;
using Coimbra.Services.Events;
using Core.Colony;
using Core.Debugger;
using Core.Units;
using Cysharp.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using System;
using System.Collections.Generic;
using System.Linq;
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

        [Header("Debug")]
        [SerializeField] private bool _debug;
        [SerializeField] private DebugChannelSO _debugChannel;

        private SimpleMultiAgentGroup _agentGroup = new();
        private HashSet<Ant> _ants = new();

        private IObjectPool<Ant> _antPool;

        private float _groupReward = 0;
        private float _addedGroupReward = 0;

        public void EndGroupEpisode()
        {
            Debug.Log($"End group episode with group reward: {_groupReward}", this);

            _groupReward = 0;

            foreach (var ant in _ants.ToList())
            {
                DespawnAnt(ant);
            }

            _agentGroup.EndGroupEpisode();
        }

        public void AddGroupReward(float value)
        {
            _groupReward += value;
            _addedGroupReward = value;
            _agentGroup.AddGroupReward(value);
        }

        private void RegisterAnt(Ant ant)
        {
            _agentGroup.RegisterAgent(ant.Agent);
            _ants.Add(ant);
            Debug.Log($"Register ant: {ant.gameObject.name}", this);
        }

        private void UnregisterAnt(Ant ant)
        {
            _agentGroup.UnregisterAgent(ant.Agent);
            _ants.Remove(ant);
            Debug.Log($"Unregister ant: {ant.gameObject.name}", this);
        }

        protected override void OnInitialize()
        {
            ServiceLocator.Set<IColonyService>(this);
            Debug.Log($"On Init");

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

                    RegisterAnt(ant);
                },
                actionOnRelease: (ant) =>
                {
                    ant.gameObject.SetActive(false);
                    ant.Agent.EndEpisode();
                    UnregisterAnt(ant);
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
            _curriculumManager.EnvironmentResetted += EnvironmentResettedEventHandler;
        }

        private void Update()
        {
            RaiseDebug();
        }

        [System.Diagnostics.Conditional(conditionString: "RAISE_DEBUG"), System.Diagnostics.Conditional(conditionString: "UNITY_EDITOR")]
        private void RaiseDebug()
        {
            if (!_debug)
                return;

            _debugChannel.RaiseEvent("colony", new ColonyDebugData()
            {
                GroupCumulativeReward = _groupReward,
                AddedGroupReward = _addedGroupReward,
                AntCount = _ants.Count,
            });

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

        private void EnvironmentResettedEventHandler()
        {
            Debug.Log($"Spawning {_initialAntCount} ants");

            foreach (var ant in _ants.ToList())
            {
                DespawnAnt(ant);
            }

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
            if (_ants.Count <= 0)
            {
                ColonyFailure().Forget();
            }
        }

        private async UniTask ColonyFailure()
        {
            //new ColonyEvent(ColonyEventType.ColonyDeath).Invoke(this);
            FindAnyObjectByType<ColonyScorer>().QueenDeath();

            await UniTask.DelayFrame(1);
            Debug.Log($"Colony failure, ants: {_ants.Count}", this);
            EndGroupEpisode();
            _curriculumManager.RestartRound();
        }

        private void ColonyEventHandler(ref EventContext context, in ColonyEvent e)
        {
            switch (e.EventType)
            {
                case ColonyEventType.QueenProcreation:
                    SpawnAnt();
                    break;
                case ColonyEventType.QueenDeath:
                    ColonyFailure().Forget();
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