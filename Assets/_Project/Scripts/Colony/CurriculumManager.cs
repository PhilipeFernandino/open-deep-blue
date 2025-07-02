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
        Queen = 251,
        Colony101 = 351,
    }

    [Serializable]
    public class LessonConfigMap { public Lesson Lesson; [Expandable] public LessonConfigSO Config; }

    [Serializable]
    public class LessonHandlerMap { public Lesson Lesson; [Expandable] public LessonHandler LessonHandler; }

    public class CurriculumManager : Actor, ICurriculumService
    {
        [SerializeField] private List<LessonHandlerMap> _lessonHandlerMap;
        [SerializeField] private Lesson _defaultLesson;

        [Header("Debugging")]
        [SerializeField] private bool _debug;
        [SerializeField] private DebugChannelSO _debugChannel;

        private CurriculumSettingsSO _settings;
        private IGridService _gridService;
        private IColonyService _colonyService;
        private readonly Dictionary<Lesson, LessonHandler> _lessonHandlers = new();
        private int _currentLessonIndex = -1;
        private int _groupStepCounter = 0;
        private int _environmentStepCounter = 0;


        public event Action EnvironmentResetted;
        public event Action EnvironmentSetup;
        public event Action MaxStepReached;

        public int CurrentLessonIndex => _currentLessonIndex;
        public Lesson CurrentLesson => (Lesson)_currentLessonIndex;
        public LessonHandler CurrentLessonHandler => _lessonHandlers[CurrentLesson];

        private bool _initialized;

        public LessonConfigSO GetCurrentConfig()
        {
            return _settings.GetConfig(CurrentLesson);
        }

        protected override void OnInitialize()
        {
            Debug.Log("CurriculumManager OnInitialize", this);

            ServiceLocator.Set<ICurriculumService>(this);

            OnStarting += CurriculumManagerOnStarting;

            foreach (var lessonMap in _lessonHandlerMap)
            {
                _lessonHandlers.Add(lessonMap.Lesson, lessonMap.LessonHandler);
            }
        }

        private void CurriculumManagerOnStarting(Actor sender)
        {
            Debug.Log("CurriculumManagerOnStarting", this);

            _colonyService = ServiceLocator.Get<IColonyService>();
            _settings = ScriptableSettings.GetOrFind<CurriculumSettingsSO>();
            _gridService = ServiceLocatorUtilities.GetServiceAssert<IGridService>();

            AntEvent.AddListener(AntEventHandler);
            ColonyEvent.AddListener(ColonyEventHandler);

            Academy.Instance.OnEnvironmentReset += OnAcademyEnvironmentReset;

            SetupLesson((int)_defaultLesson);

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
            int newLessonIndex = (int)Academy.Instance.EnvironmentParameters.GetWithDefault("lesson_index", (float)_defaultLesson);
            _currentLessonIndex = newLessonIndex;
            ResetEnvironment();
        }

        private void SetupLesson(int lesson)
        {
            Debug.Log($"Setup lesson {lesson}", this);
            _currentLessonIndex = lesson;
            CurrentLessonHandler.Setup(_gridService, GetCurrentConfig());
            CurrentLessonHandler.OnEnter();
        }

        private void FixedUpdate()
        {
            if (!IsStarted || _currentLessonIndex == -1)
                return;

            _groupStepCounter++;
            _environmentStepCounter++;

            int currentLessonMaxSteps = GetCurrentConfig().MaxStepsPerGroupEpisde;
            int maxStepPerEnvironmentReset = GetCurrentConfig().MaxStepPerEnvironmentReset;

            if (currentLessonMaxSteps > 0 && _groupStepCounter >= currentLessonMaxSteps)
            {
                RestartRound();
            }

            if (maxStepPerEnvironmentReset > 0 && _environmentStepCounter >= maxStepPerEnvironmentReset)
            {
                _environmentStepCounter = 0;
                SetupLesson(CurrentLessonIndex);
            }
        }

        public void RestartRound()
        {
            _groupStepCounter = 0;
            ResetEnvironment();
        }

        private void ResetEnvironment()
        {
            _environmentStepCounter = 0;
            SetupLesson(CurrentLessonIndex);
            EnvironmentResetted?.Invoke();
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
                    LessonConfig = GetCurrentConfig(),
                    Step = _groupStepCounter,
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
        public event Action MaxStepReached;
        public event Action EnvironmentSetup;
        public event Action EnvironmentResetted;
        public LessonConfigSO GetCurrentConfig();
        public int CurrentLessonIndex { get; }
    }
}
