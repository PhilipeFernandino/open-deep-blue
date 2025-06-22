using Coimbra;
using Coimbra.Services;
using Coimbra.Services.Events;
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

            AntEvent.AddListener(AntEventHandler);
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
        }

        private void AntEventHandler(ref EventContext context, in AntEvent e)
        {
            if (CurrentLesson == Lesson.Foraging101 && e.AntEventType == AntEventType.Eat)
            {
                e.Ant.Agent.EndEpisode();
            }
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
