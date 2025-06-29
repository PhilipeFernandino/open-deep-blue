using Coimbra;
using Coimbra.Services;
using Coimbra.Services.Events;
using Core.Colony.Lessons;
using Core.Debugger;
using Core.Level;
using Core.Train;
using Core.Util;
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
        Foraging102 = 51,
        Foraging103 = 52,
        Fungus = 151,
        Queen = 251
    }

    [Serializable]
    public class LessonConfigMap { public Lesson Lesson; [Expandable] public LessonConfigSO Config; }

    public class CurriculumManager : Actor, ICurriculumService
    {
        [SerializeField] private bool _debug;
        [SerializeField] private DebugChannelSO _debugChannel;

        private CurriculumSettingsSO _settings;

        private IGridService _gridService;
        private IColonyService _colonyService;

        private int _currentLessonIndex = -1;

        private int _groupStepCounter = 0;
        private int _environmentStepCounter = 0;

        public event Action EnvironmentResetted;
        public event Action EnvironmentSetup;

        public int CurrentLessonIndex => _currentLessonIndex;
        public Lesson CurrentLesson => (Lesson)_currentLessonIndex;

        public LessonHandler CurrentLessonHandler => _lessonHandlers[CurrentLesson];

        private Dictionary<Lesson, LessonHandler> _lessonHandlers;

        public LessonConfigSO GetCurrentConfig()
        {
            return _settings.GetConfig(CurrentLesson);
        }

        protected override void OnInitialize()
        {
            Debug.Log("CurriculumManager OnInitialize", this);

            ServiceLocator.Set<ICurriculumService>(this);

            OnStarting += CurriculumManagerOnStarting;
        }

        private void CurriculumManagerOnStarting(Actor sender)
        {
            Debug.Log("CurriculumManagerOnStarting", this);

            _colonyService = ServiceLocator.Get<IColonyService>();
            _settings = ScriptableSettings.GetOrFind<CurriculumSettingsSO>();
            _gridService = ServiceLocatorUtilities.GetServiceAssert<IGridService>();

            _lessonHandlers = new Dictionary<Lesson, LessonHandler>
            {
                {
                    Lesson.Foraging101,
                    new ForagingLessonHandler(_gridService, _settings.GetConfig(Lesson.Foraging101))
                },
                {
                    Lesson.Foraging102,
                    new ForagingLessonHandler(_gridService, _settings.GetConfig(Lesson.Foraging102))
                },
                {
                    Lesson.Foraging103,
                    new ForagingLessonHandler(_gridService, _settings.GetConfig(Lesson.Foraging103))
                },
                {
                    Lesson.Fungus,
                    new FungusLessonHandler(_gridService, _settings.GetConfig(Lesson.Fungus))
                },
                {
                    Lesson.Queen,
                    new QueenLessonHandler(_gridService, _settings.GetConfig(Lesson.Queen))
                },
            };

            SetupLesson(0);

            AntEvent.AddListener(AntEventHandler);
            ColonyEvent.AddListener(ColonyEventHandler);

            Academy.Instance.OnEnvironmentReset += OnAcademyEnvironmentReset;

            EnvironmentSetup?.Invoke();
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
            Debug.Log("OnAcademyEnvironmentReset", this);

            int newLessonIndex = (int)Academy.Instance.EnvironmentParameters.GetWithDefault("lesson_index", 0.0f);

            if (newLessonIndex == _currentLessonIndex)
            {
                return;
            }

            SetupLesson(newLessonIndex);
            EnvironmentResetted?.Invoke();
        }

        private void SetupLesson(int lesson)
        {
            Debug.Log($"Setup lesson {lesson}", this);
            _currentLessonIndex = lesson;
            CurrentLessonHandler.OnEnter();
        }

        private void FixedUpdate()
        {
            if (!IsStarted)
                return;

            _groupStepCounter++;
            _environmentStepCounter++;

            int currentLessonMaxSteps = GetCurrentConfig().MaxStepsPerGroupEpisde;
            int maxStepPerEnvironmentReset = GetCurrentConfig().MaxStepPerEnvironmentReset;

            if (currentLessonMaxSteps > 0 && _groupStepCounter >= currentLessonMaxSteps)
            {
                ResetRound();
            }

            if (maxStepPerEnvironmentReset > 0 && _environmentStepCounter >= maxStepPerEnvironmentReset)
            {
                ResetEnvironment();
            }
        }

        private void ResetEnvironment()
        {
            EnvironmentResetted?.Invoke();
            _environmentStepCounter = 0;
            SetupLesson(CurrentLessonIndex);
        }

        private void ResetRound()
        {
            _groupStepCounter = 0;
            _colonyService.EndGroupEpisode();
            ResetEnvironment();
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

            _debugChannel.RaiseEvent("curriculum",
                new CurriculumDebugData()
                {
                    CurrentLesson = CurrentLesson,
                    LessonConfig = GetCurrentConfig()
                }
             );
        }

        private void AntEventHandler(ref EventContext context, in AntEvent e)
        {
            if (_currentLessonIndex == -1)
            {
                return;
            }

            CurrentLessonHandler.HandleAntEvent(e);
        }

        private void ColonyEventHandler(ref EventContext context, in ColonyEvent e)
        {
            if (_currentLessonIndex == -1)
            {
                return;
            }

            CurrentLessonHandler.HandleColonyEvent(e);
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
