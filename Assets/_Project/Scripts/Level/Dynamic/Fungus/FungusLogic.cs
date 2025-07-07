using Core.RL;
using System;
using UnityEngine;

namespace Core.Level.Dynamic
{
    public class FungusLogic : ILogicController<FungusData, FungusDefinition>
    {
        public void OnUpdate(ref FungusData data, FungusDefinition defData, Vector2Int position, IGridService grid, IChemicalGridService chemicals)
        {
            if (data.CurrentSaciation <= 0)
            {
                data.CurrentHealth = Mathf.Max(
                    data.CurrentHealth - (defData.LostHealthWhenStarved * Time.fixedDeltaTime),
                    0
                );
            }
            else
            {
                if (data.CurrentSaciation > defData.MaxSaciation * defData.FoodProductionSaciationThreshold)
                {
                    data.CurrentHealth = Mathf.Min(
                        data.CurrentHealth + (defData.LostHealthWhenStarved * Time.fixedDeltaTime),
                        defData.MaxHealth
                    );

                    data.CurrentFoodStore = Mathf.Min(
                        data.CurrentFoodStore + (defData.FoodProduction * Time.fixedDeltaTime),
                        defData.MaxFoodStore
                    );

                    new ColonyEvent(ColonyEventType.FungusProducing).Invoke(data);
                }

                data.CurrentSaciation = Mathf.Max(
                    data.CurrentSaciation - (defData.SaciationLost * Time.fixedDeltaTime),
                    0f
                );
            }

            if (data.CurrentHealth <= 0)
            {
                new ColonyEvent(ColonyEventType.FungusDeath).Invoke(data);
                data.CurrentHealth = defData.MaxHealth;
            }
        }
    }
}