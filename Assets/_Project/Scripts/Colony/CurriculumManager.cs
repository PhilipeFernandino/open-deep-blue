using Coimbra;
using Coimbra.Services;
using Core.Level;
using Core.Map;
using Core.Train;
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
        private CurriculumSettingsSO _settings;

        private IGridService _gridService;

        private int _currentLessonIndex = -1;

        public event Action EnvironmentResetted;

        public int CurrentLessonIndex => _currentLessonIndex;
        public Lesson CurrentLesson => (Lesson)_currentLessonIndex;

        public LessonConfigSO GetCurrentConfig()
        {
            return _settings.GetConfig(CurrentLesson);
        }

        protected override void OnInitialize()
        {
            Academy.Instance.OnEnvironmentReset += OnAcademyEnvironmentReset;
            ServiceLocator.Set<ICurriculumService>(this);
        }

        protected override void OnSpawn()
        {
            _settings = ScriptableSettings.GetOrFind<CurriculumSettingsSO>();
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

            switch (_currentLessonIndex)
            {
                case 0:
                    SetupForagingLesson();
                    break;
                case 1:
                    //SetupFungusLesson();
                    break;
                case 2:
                    //SetupQueenLesson();
                    break;
                default:
                    //SetupForagingLesson();
                    break;
            }

            EnvironmentResetted?.Invoke();
        }

        private void SetupForagingLesson()
        {
            //_gridService.ListPositions(Tile.GreenGrass, _foodSpawnPoints);
            //_gridService.ListPositions(Tile.AntQueenSpawn, _antSpawnPoints);
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
