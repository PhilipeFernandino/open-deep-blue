using Core.Map;
using System.Text;
using UnityEngine;

namespace Core.Debugger
{
    public struct GridDebugData
    {
        public int Dimensions;
        public Vector2Int TilePosition;
        public TileInstance TileInstance;
        public TileDefinition TileDefiniton;
    }

    [CreateAssetMenu(fileName = "Grid Info Module", menuName = "Core/Debugger/Modules/Grid Info Module")]
    public class GridInfoModule : DebugModuleSO
    {
        private readonly StringBuilder _stringBuilder = new StringBuilder();

        public override void UpdateData(object data)
        {
            if (data is GridDebugData tileData)
            {
                _stringBuilder.Clear();

                _stringBuilder.AppendLine($"Grid Dim: ({tileData.Dimensions} x {tileData.Dimensions})");
                _stringBuilder.AppendLine($"Mouse At Grid Position: {tileData.TilePosition}");
                _stringBuilder.AppendLine($"Tile Instance: {tileData.TileInstance}");
                _stringBuilder.AppendLine($"Tile Definition: {tileData.TileDefiniton}");

                DisplayText = _stringBuilder.ToString();
            }
        }

        public override void ResetData()
        {
            DisplayText = "Out of bounds.";
        }
    }
}