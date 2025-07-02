using Core.Level;
using Core.Train;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using Unity.MLAgents;
using UnityEngine;

namespace Core.Colony
{
    public abstract class LessonHandler : ScriptableObject
    {
        protected IGridService GridService;
        protected LessonConfigSO Config;
        protected List<Vector2Int> AntSpawnPoints = new();

        protected IColonyService _colonyService;

        public void Setup(IGridService gridService, LessonConfigSO config)
        {
            GridService = gridService;
            Config = config;
        }

        protected async void EndAgentEpisodeNextFrame(Agent agent)
        {
            await UniTask.DelayFrame(1);
            _colonyService.EndAgentEpisode(agent);
        }

        public abstract void OnEnter();

        public abstract void OnExit();

        public abstract void HandleAntEvent(in AntEvent e);
        public abstract void HandleColonyEvent(in ColonyEvent e);

        public abstract Vector2 GetSpawnPoint();
    }
}