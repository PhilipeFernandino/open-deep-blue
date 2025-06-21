using Core.ItemSystem;
using Core.Map;
using System.Text;
using UnityEngine;

namespace Core.Debugger
{
    public struct AntDebugData
    {
        public Vector2 Position;
        public Item CarryingItem;
        public Tile FacingTile;
        public float Saciety;
        public float MaxSaciety;
        public float Health;
        public float MaxHealth;
    }

    [CreateAssetMenu(fileName = "Ant Info Module", menuName = "Core/Debugger/Modules/Ant Info Module")]
    public class AntInfoModule : DebugModuleSO
    {
        private readonly StringBuilder _stringBuilder = new StringBuilder();

        public override void UpdateData(object data)
        {
            if (data is AntDebugData antData)
            {
                _stringBuilder.Clear();

                _stringBuilder.AppendLine($"Position: ({antData.Position})");
                _stringBuilder.AppendLine($"Carrying Item: {antData.CarryingItem}");
                _stringBuilder.AppendLine($"Facing Tile: {antData.FacingTile}");
                _stringBuilder.AppendLine($"Saciety: {antData.Saciety}/{antData.MaxSaciety}");
                _stringBuilder.AppendLine($"Saciety: {antData.Health}/{antData.MaxHealth}");

                DisplayText = _stringBuilder.ToString();
            }
        }

        public override void ResetData()
        {
            DisplayText = "No data.";
        }
    }
}