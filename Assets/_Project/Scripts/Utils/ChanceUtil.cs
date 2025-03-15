using UnityEngine;

namespace Core.Util
{
    using static Random;
    public static class ChanceUtil
    {
        /// <summary>
        /// Returns a event considering one experiment with the passed <paramref name="chance"/>. 
        /// Chance should be between 0f and 1f (not enforced).
        /// </summary>
        public static bool EventSuccess(float chance)
        {
            float randomValue = UnityEngine.Random.Range(0f, 1f);
            return chance >= randomValue;
        }

        public static bool EventSuccess(float chance, System.Random rng)
        {
            float randomValue = Range(rng, 0f, 1f);
            return chance >= randomValue;
        }
    }
}
