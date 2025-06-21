using Core.Train;
using UnityEngine;

namespace Core.Level.Dynamic
{
    public class QueenLogic
    {
        public void OnUpdate(ref QueenData data, QueenDefinition defData, Vector2Int position, IChemicalGridService chemicals)
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
                if (data.CurrentSaciation > defData.MaxSaciation * defData.PregnancySaciationThreshold)
                {
                    data.CurrentHealth = Mathf.Min(
                        data.CurrentHealth + (defData.LostHealthWhenStarved * Time.fixedDeltaTime),
                        defData.MaxHealth
                    );
                }

                data.CurrentSaciation = Mathf.Max(
                    0f,
                    data.CurrentSaciation - (defData.SaciationLost * Time.fixedDeltaTime)
                );
            }

            if (data.CurrentPregnancyPercentage >= 1f)
            {
                new ColonyEvent(ColonyEventType.QueenProcreation).Invoke(data);
                data.CurrentPregnancyPercentage = 0f;
            }

            data.CurrentPregnancyPercentage += defData.PregnancyRate;

            if (data.CurrentHealth <= 0)
            {
                new ColonyEvent(ColonyEventType.QueenDeath).Invoke(data);
                data.CurrentHealth = defData.MaxHealth;
            }

        }
    }
}