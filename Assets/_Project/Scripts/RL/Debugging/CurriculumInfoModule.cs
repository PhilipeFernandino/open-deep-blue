using Core.RL;
using System.Text;
using UnityEngine;

namespace Core.Debugger.RL
{
    public struct CurriculumDebugData
    {
        public Lesson CurrentLesson;
        public LessonConfigSO LessonConfig;
        public int Step;
    }

    [CreateAssetMenu(fileName = "Curriculum Info Module", menuName = "Core/Debugger/Modules/Curriculum Info Module")]
    public class CurriculumInfoModule : DebugModuleSO
    {
        private readonly StringBuilder _stringBuilder = new StringBuilder();

        public override void UpdateData(object data)
        {
            if (data is CurriculumDebugData curriculumDebugData)
            {
                _stringBuilder.Clear();

                _stringBuilder.AppendLine($"Current Lesson: {curriculumDebugData.CurrentLesson}");
                _stringBuilder.AppendLine($"Steps: {curriculumDebugData.Step}");

                foreach (var antEventReward in curriculumDebugData.LessonConfig.AntEvents)
                {
                    _stringBuilder.AppendLine($"{antEventReward.EventType}: {antEventReward.Reward}");
                }

                foreach (var colonyEventReward in curriculumDebugData.LessonConfig.ColonyEvents)
                {
                    _stringBuilder.AppendLine($"{colonyEventReward.EventType}: {colonyEventReward.Reward}");
                }

                DisplayText = _stringBuilder.ToString();
            }
        }

        public override void ResetData()
        {
            DisplayText = "No data.";
        }
    }
}