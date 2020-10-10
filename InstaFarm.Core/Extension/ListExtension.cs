using System;
using System.Collections.Generic;

namespace InstaFarm.Core.Extension
{
    public static class ListExtension
    {
        public static T ChooseRandom<T>(this List<T> list)
        {
            if (list.Count <= 0) return default(T);

            var rnd = new Random();
            return list[rnd.Next(0, list.Count)];
        }
    }
}