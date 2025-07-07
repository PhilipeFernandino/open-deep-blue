using System.Text;
using UnityEngine;

namespace Core.Debugger.RL
{
    public struct ColonyDebugData
    {
        public float GroupCumulativeReward;
        public float AddedGroupReward;
        public int AntCount;
    }


    [CreateAssetMenu(fileName = "Colony Info Module", menuName = "Core/Debugger/Modules/Colony Info Module")]
    public class ColonyInfoModuleSO : DebugModuleSO
    {
        private readonly StringBuilder _stringBuilder = new StringBuilder();

        public override void UpdateData(object data)
        {
            if (data is ColonyDebugData debugData)
            {
                _stringBuilder.Clear();
                _stringBuilder.AppendLine($"Group Cumulative Reward: {debugData.GroupCumulativeReward}");
                _stringBuilder.AppendLine($"Added Group Reward: {debugData.AddedGroupReward}");
                _stringBuilder.AppendLine($"Ant Count: {debugData.AntCount}");
                DisplayText = _stringBuilder.ToString();
            }
        }

        public override void ResetData()
        {
            DisplayText = "No data.";
        }
    }
}