using UnityEngine;

namespace Core.Level.Dynamic
{
    [CreateAssetMenu(menuName = "Core/Dynamic/Queen Definition")]
    public class QueenDefinition : ScriptableObject
    {
        public float MaxHealth;
        public float MaxSaciation;
        public float SaciationLost;
        public float LostHealthWhenStarved;
        public float PregnancyRate;
        public float PregnancySaciationThreshold;
        public int BroodPerLaying;
    }
}