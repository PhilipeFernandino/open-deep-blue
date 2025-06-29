namespace Core.Colony
{
    using Core.Level;
    using Core.Train;
    using Cysharp.Threading.Tasks;
    using System;
    using System.Collections.Generic;
    using Unity.MLAgents;
    using UnityEngine;

    public abstract class LessonHandler
    {
        protected readonly IGridService GridService;
        protected readonly LessonConfigSO Config;
        protected readonly List<Vector2Int> AntSpawnPoints;

        private IColonyService _colonyService;

        protected LessonHandler(IGridService gridService, LessonConfigSO config)
        {
            GridService = gridService;
            Config = config;
            AntSpawnPoints = new List<Vector2Int>();
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