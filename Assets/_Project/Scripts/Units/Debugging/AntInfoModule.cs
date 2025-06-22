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
        public float CumulativeReward;

        public bool CanEat;
        public bool CanDig;
        public bool CanFeedFungus;
        public bool CanFeedQueen;
        public bool CanGatherLeaf;
        public bool CanGatherFungus;
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

                _stringBuilder.AppendLine($"Position: {antData.Position}");
                _stringBuilder.AppendLine($"Carrying Item: {antData.CarryingItem}");
                _stringBuilder.AppendLine($"Facing Tile: {antData.FacingTile}");
                _stringBuilder.AppendLine($"Saciety: {antData.Saciety:0.##}/{antData.MaxSaciety}");
                _stringBuilder.AppendLine($"Saciety: {antData.Health:0.##}/{antData.MaxHealth}");

                if (antData.CanEat)
                    _stringBuilder.AppendLine("CanEat");
                if (antData.CanDig)
                    _stringBuilder.AppendLine("CanDig");
                if (antData.CanFeedFungus)
                    _stringBuilder.AppendLine("CanFeedFungus");
                if (antData.CanFeedQueen)
                    _stringBuilder.AppendLine("CanFeedQueen");
                if (antData.CanGatherLeaf)
                    _stringBuilder.AppendLine("CanGatherLeaf");
                if (antData.CanGatherFungus)
                    _stringBuilder.AppendLine("CanGatherFungus");

                DisplayText = _stringBuilder.ToString();
            }
        }

        public override void ResetData()
        {
            DisplayText = "No data.";
        }
    }
}