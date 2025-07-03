using UnityEngine;

namespace Core.HealthSystem
{
    [CreateAssetMenu(menuName = "Core/Health Data")]
    public class HealthData : ScriptableObject
    {
        [field: SerializeField] public float BaseHealth { get; private set; }
    }
}