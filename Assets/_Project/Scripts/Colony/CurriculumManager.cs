using Coimbra;
using Coimbra.Services;
using Coimbra.Services.Events;
using Core.Debugger;
using Core.Level;
using Core.Map;
using Core.Train;
using Core.Util;
using Extensions;
using NaughtyAttributes;
using System;
using System.Collections.Generic;
using Unity.MLAgents;
using UnityEngine;

namespace Core.Colony
{
    public enum Lesson
    {
        Foraging101 = 0,
        Fungus = 1,
        Queen = 2,
    }

    [Serializable]
    public class LessonConfigMap { public Lesson Lesson; public LessonConfigSO Config; }

    public class CurriculumManager : Actor, ICurriculumService
    {
        [SerializeField] private bool _debug;
        [SerializeField] private DebugChannelSO _debugChannel;

        private CurriculumSettingsSO _settings;

        private IGridService _gridService;

        private int _currentLessonIndex = -1;

        public event Action EnvironmentResetted;

        public int CurrentLessonIndex => _currentLessonIndex;
        public Lesson CurrentLesson => (Lesson)_currentLessonIndex;

        private List<Vector2Int> _spawnPoints = new();
        private List<Vector2Int> _greenGrassPoints = new();

        public LessonConfigSO GetCurrentConfig()
        {
            return _settings.GetConfig(CurrentLesson);
        }

        protected override void OnInitialize()
        {
            Academy.Instance.OnEnvironmentReset += OnAcademyEnvironmentReset;
            ServiceLocator.Set<ICurriculumService>(this);

            AntEvent.AddListener(AntEventHandler);
        }

        protected override void OnSpawn()
        {
            _settings = ScriptableSettings.GetOrFind<CurriculumSettingsSO>();
            _gridService = ServiceLocatorUtilities.GetServiceAssert<IGridService>();
        }

        protected override void OnDestroyed()
        {
            if (Academy.IsInitialized)
            {
                Academy.Instance.OnEnvironmentReset -= OnAcademyEnvironmentReset;
            }
        }

        private void OnAcademyEnvironmentReset()
        {
            int newLessonIndex = (int)Academy.Instance.EnvironmentParameters.GetWithDefault("lesson_index", 0.0f);

            if (newLessonIndex == _currentLessonIndex)
            {
                return;
            }

            _currentLessonIndex = newLessonIndex;
            SetupLesson(_currentLessonIndex);
            EnvironmentResetted?.Invoke();
        }

        private void LoadCurrentMapConfig()
        {
            var tilemapAsset = GetCurrentConfig().Tilemap;
            var mapMetadata = new MapMetadata(
                tilemapAsset.Tiles,
                tilemapAsset.BiomeTiles,
                new List<PointOfInterest>(),
                tilemapAsset.Dimensions,
                tilemapAsset.Name
            );

            new MapMetadataGeneratedEvent(mapMetadata).Invoke(this);
        }

        private void SetupLesson(int lesson)
        {
            switch (lesson)
            {
                case 0:
                    SetupForagingLesson();
                    break;
                case 1:
                    SetupFungusLesson();
                    break;
                case 2:
                    SetupQueenLesson();
                    break;
                default:
                    break;
            }
        }

        private void Update()
        {
            if (!_debug)
                return;

            _debugChannel.RaiseEvent("curriculum",
                new CurriculumDebugData()
                {
                    CurrentLesson = CurrentLesson,
                    LessonConfig = GetCurrentConfig()
                }
             );
        }

        private void SetupForagingLesson()
        {
            LoadCurrentMapConfig();
            _gridService.ListPositions(Tile.AntQueenSpawn, _spawnPoints);
        }

        private void SetupFungusLesson()
        {
            LoadCurrentMapConfig();
            _gridService.ListPositions(Tile.AntQueenSpawn, _spawnPoints);

        }

        private void SetupQueenLesson()
        {
            LoadCurrentMapConfig();
            _gridService.ListPositions(Tile.AntQueenSpawn, _spawnPoints);
        }

        private void AntEventHandler(ref EventContext context, in AntEvent e)
        {
            switch (e.AntEventType)
            {
                case AntEventType.Born:
                    e.Ant.Agent.SpawnPointRequested += AgentSpawnPointRequestedEventHandler;
                    break;
                case AntEventType.Death:
                    e.Ant.Agent.SpawnPointRequested -= AgentSpawnPointRequestedEventHandler;
                    break;
            }

            if (CurrentLesson == Lesson.Foraging101)
            {
                if (e.AntEventType == AntEventType.Eat)
                {
                    e.Ant.Agent.EndEpisode();
                }
            }
        }

        private Vector2 AgentSpawnPointRequestedEventHandler()
        {
            return _spawnPoints.RandomElement();
        }

        [Button]
        private void NextLesson()
        {
            _currentLessonIndex = (_currentLessonIndex + 1) % 3;
            SetupLesson(_currentLessonIndex);
        }
    }

    [DynamicService]
    public interface ICurriculumService : IService
    {
        public LessonConfigSO GetCurrentConfig();

        public event Action EnvironmentResetted;
        public int CurrentLessonIndex { get; }
    }
}
