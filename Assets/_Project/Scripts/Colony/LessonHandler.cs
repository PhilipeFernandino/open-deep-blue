namespace Core.Colony
{
    using Core.Level;
    using Core.Train;
    using System.Collections.Generic;
    using UnityEngine;

    public abstract class LessonHandler
    {
        protected readonly IGridService GridService;
        protected readonly LessonConfigSO Config;
        protected readonly List<Vector2Int> AntSpawnPoints;

        protected LessonHandler(IGridService gridService, LessonConfigSO config)
        {
            GridService = gridService;
            Config = config;
            AntSpawnPoints = new List<Vector2Int>();
        }

        public abstract void OnEnter();

        public abstract void OnExit();

        public abstract void HandleAntEvent(in AntEvent e);
        public abstract void HandleColonyEvent(in ColonyEvent e);

        public abstract Vector2 GetSpawnPoint();
    }
}