using Core.Level.Dynamic;
using System.Text;
using UnityEngine;

namespace Core.Debugger
{
    public struct FungusTileData
    {
        public float CurrentSaciation;
        public float MaxSaciation;
        public float SaciationLost;
        public float CurrentFoodStore;
        public float CurrentHealth;
        public float MaxFoodStore;
        public float MaxHealth;
        public float LostHealthWhenStarved;
        public float FoodProduction;
    }

    public struct QueenTileData
    {
        public QueenData QueenData;
        public QueenDefinition QueenDefinition;
    }

    public struct FoodTileData
    {
        public float CurrentFoodStore;
        public float MaxFoodStore;
    }

    [CreateAssetMenu(fileName = "Dynamic Grid Info Module", menuName = "Core/Debugger/Modules/Dynamic Grid Info Module")]
    public class DynamicGridInfoModule : DebugModuleSO
    {
        private readonly StringBuilder _stringBuilder = new StringBuilder();

        public override void UpdateData(object data)
        {
            _stringBuilder.Clear();

            if (data is FungusTileData tileData)
            {
                _stringBuilder.AppendLine($"Fungus Tile:");
                _stringBuilder.AppendLine($"Health: {tileData.CurrentHealth}/{tileData.MaxHealth}");
                _stringBuilder.AppendLine($"Saciation: {tileData.CurrentSaciation}/{tileData.MaxSaciation}");
                _stringBuilder.AppendLine($"Food Store: {tileData.CurrentFoodStore}/{tileData.MaxFoodStore}");
                _stringBuilder.AppendLine($"Lost Health When Starved: {tileData.LostHealthWhenStarved}");
                _stringBuilder.AppendLine($"Food Production: {tileData.FoodProduction}");
                _stringBuilder.AppendLine($"Lost Saciation: {tileData.SaciationLost}");

            }

            if (data is QueenTileData queenTileData)
            {
                _stringBuilder.AppendLine($"Queen Tile:");
                _stringBuilder.AppendLine($"Health: {queenTileData.QueenData.CurrentHealth}/{queenTileData.QueenDefinition.MaxHealth}");
                _stringBuilder.AppendLine($"Saciation: {queenTileData.QueenData.CurrentSaciation} / {queenTileData.QueenDefinition.MaxSaciation}");
                _stringBuilder.AppendLine($"Pregnancy: {queenTileData.QueenData.CurrentPregnancyPercentage * 100f}%");
                _stringBuilder.AppendLine($"Lost Health When Starved: {queenTileData.QueenDefinition.LostHealthWhenStarved}");
                _stringBuilder.AppendLine($"Brood Per Laying: {queenTileData.QueenDefinition.BroodPerLaying}");
            }

            if (data is FoodTileData foodTileData)
            {
                _stringBuilder.AppendLine($"Food Tile:");
                _stringBuilder.AppendLine($"Food: {foodTileData.CurrentFoodStore}/{foodTileData.MaxFoodStore}");
            }


            DisplayText = _stringBuilder.ToString();
        }

        public override void ResetData()
        {
            DisplayText = "No data.";
        }
    }
}