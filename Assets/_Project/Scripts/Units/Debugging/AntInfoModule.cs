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

        public float Energy;
        public float MaxEnergy;

        public bool CanEat;
        public bool CanDig;
        public bool CanFeedFungus;
        public bool CanFeedQueen;
        public bool CanGatherLeaf;
        public bool CanGatherFungus;

        public Vector2 QueenDirection;
        public Vector2 FungusDirection;
        public Vector2 FoodDirection;

        public Vector2 MovingDirection;
    }

    [CreateAssetMenu(fileName = "Ant Info Module", menuName = "Core/Debugger/Modules/Ant Info Module")]
    public class AntInfoModule : DebugModuleSO
    {
        private readonly StringBuilder _stringBuilder = new StringBuilder();

        public override void UpdateData(object data)
        {
            string bothAndPercentage(float value1, float value2)
            {
                return $"{value1,5:0.00}/{value2,-5:0.00} - {(value1 / value2 * 100f):0.00}%";
            }

            if (data is AntDebugData antData)
            {
                _stringBuilder.Clear();

                _stringBuilder.AppendLine($"Position: {antData.Position}");
                _stringBuilder.AppendLine($"Carrying Item: {antData.CarryingItem}");
                _stringBuilder.AppendLine($"Facing Tile: {antData.FacingTile}");
                _stringBuilder.AppendLine($"Saciety: {bothAndPercentage(antData.Saciety, antData.MaxSaciety)}");
                _stringBuilder.AppendLine($"Health: {bothAndPercentage(antData.Health, antData.MaxHealth)}");
                _stringBuilder.AppendLine($"Energy: {bothAndPercentage(antData.Energy, antData.MaxEnergy)}");
                _stringBuilder.AppendLine($"Moving Direction: {antData.MovingDirection}");
                _stringBuilder.AppendLine($"Queen Direction: {antData.QueenDirection}");
                _stringBuilder.AppendLine($"Food Direction: {antData.FoodDirection}");
                _stringBuilder.AppendLine($"Fungus Direction: {antData.FungusDirection}");


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