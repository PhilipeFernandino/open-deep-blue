using UnityEngine;

namespace Core.Level.Dynamic
{
    public class QueenLogic : ILogicController<QueenData, QueenDefinition>
    {
        public void OnUpdate(ref QueenData data, QueenDefinition defData, Vector2Int position, IGridService grid, IChemicalGridService chemicals)
        {
            if (data.CurrentSaciation <= 0)
            {
                data.CurrentHealth -= defData.LostHealthWhenStarved;
            }
            else
            {
                if (data.CurrentSaciation > defData.MaxSaciation * 0.2)
                {
                    data.CurrentHealth += defData.LostHealthWhenStarved;
                }

                data.CurrentSaciation = Mathf.Max(0f, data.CurrentSaciation - defData.SaciationLost);
            }

            if (data.CurrentPregnancyPercentage >= 1f)
            {
                // TODO: lay eggs
                data.CurrentPregnancyPercentage = 0f;
            }

            data.CurrentPregnancyPercentage += defData.PregnancyRate;

            if (data.CurrentHealth <= 0)
            {
                // TODO: 
                //grid.TrySetTileAt(position, Map.Tile.None);
            }

        }
    }
}