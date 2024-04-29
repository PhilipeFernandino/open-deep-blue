using UnityEngine;

namespace Core.Utils
{
    public static class ChanceUtil
    {
        /// <summary>
        /// Returns a event considering one experiment with the passed <paramref name="chance"/>. 
        /// Chance should be between 0f and 1f (not enforced).
        /// </summary>
        public static bool EventSuccess(float chance)
        {
            float randomValue = Random.Range(0f, 1f);
            return chance >= randomValue;
        }
    }
}
