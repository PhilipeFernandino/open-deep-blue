using UnityEngine;

namespace Core.Level.Dynamic
{
    public class FungusLogic : ILogicController<FungusData, FungusDefinition>
    {
        public void OnUpdate(ref FungusData data, FungusDefinition defData, Vector2Int position, IGridService grid, IChemicalGridService chemicals)
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
                    data.CurrentFoodStore += defData.FoodProduction;
                }

                data.CurrentSaciation = Mathf.Max(0f, data.CurrentSaciation - defData.SaciationLost);
            }

            if (data.CurrentHealth <= 0)
            {
                // TODO: 
                //grid.TrySetTileAt(position, Map.Tile.None);
            }

        }
    }
}