using UnityEngine;

namespace Core.Unit
{
    [CreateAssetMenu(menuName = "Core/Dynamic/Ant Definition")]
    public class AntDefinitionSO : ScriptableObject
    {
        public float MaxSaciety = 50;
        public float AttackDistance = 1;
        public float AttackDamage = 5;
        public float AggroDistance = 1;
        public float DigDamage = 5;
        public float MovementSpeed = 2;
        public float SacietyLoss = 1;
    }
}