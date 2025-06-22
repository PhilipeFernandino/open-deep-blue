using Coimbra;
using Core.Colony;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Train
{
    [ProjectSettings("Core")]
    public class CurriculumSettingsSO : ScriptableSettings
    {
        [SerializeField] private List<LessonConfigMap> _configMap = new();

        private Dictionary<Lesson, LessonConfigSO> _lessonConfig = new();

        public LessonConfigSO GetConfig(Lesson lesson)
        {
            _lessonConfig.TryGetValue(lesson, out var config);
            return config;
        }

        protected override void OnLoaded()
        {
            _lessonConfig.Clear();

            foreach (var config in _configMap)
            {
                _lessonConfig.Add(config.Lesson, config.Config);
            }
        }
    }
}