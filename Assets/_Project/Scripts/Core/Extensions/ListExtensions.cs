using System;
using System.Collections.Generic;

namespace Extensions
{
    public static class ListExtensions
    {
        private static readonly Random Rng = new Random();

        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;

            while (n > 1)
            {
                n--;
                int k = Rng.Next(n + 1);
                (list[k], list[n]) = (list[n], list[k]);
            }
        }

        public static T RandomElement<T>(this IList<T> list)
        {
            int n = list.Count;
            return list[UnityEngine.Random.Range(0, n)];
        }
    }
}