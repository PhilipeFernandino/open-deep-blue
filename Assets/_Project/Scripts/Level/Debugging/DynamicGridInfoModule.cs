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

    [CreateAssetMenu(fileName = "Dynamic Grid Info Module", menuName = "Core/Debugger/Modules/Dynamic Grid Info Module")]
    public class DynamicGridInfoModule : DebugModuleSO
    {
        private readonly StringBuilder _stringBuilder = new StringBuilder();

        public override void UpdateData(object data)
        {
            if (data is FungusTileData tileData)
            {
                _stringBuilder.Clear();

                _stringBuilder.AppendLine($"Fungus Tile:");
                _stringBuilder.AppendLine($"CurrentHealth: {tileData.CurrentHealth}/{tileData.MaxHealth}");
                _stringBuilder.AppendLine($"CurrentSaciation: {tileData.CurrentSaciation}/{tileData.MaxSaciation}");
                _stringBuilder.AppendLine($"CurrentFoodStore: {tileData.CurrentFoodStore}/{tileData.MaxFoodStore}");
                _stringBuilder.AppendLine($"LostHealthWhenStarved: {tileData.LostHealthWhenStarved}");
                _stringBuilder.AppendLine($"FoodProduction: {tileData.FoodProduction}");
                _stringBuilder.AppendLine($"LostSaciation: {tileData.SaciationLost}");

                DisplayText = _stringBuilder.ToString();
            }
        }

        public override void ResetData()
        {
            DisplayText = "No data.";
        }
    }
}