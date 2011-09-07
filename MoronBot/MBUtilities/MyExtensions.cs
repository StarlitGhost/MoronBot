using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace MBUtilities
{
    public static class MyExtensions
    {
        public static int FindIndex<T>(this BindingList<T> list, Predicate<T> match)
        {
            int index = -1;
            for (int i = 0; i < list.Count; i++)
                if (match(list[i]))
                {
                    index = i;
                    break;
                }
            return index;
        }

        public static void Shuffle<T>(this IList<T> list)
        {
            Random rand = new Random();
            for (int i = list.Count - 1; i > 0; i--)
            {
                int n = rand.Next(i + 1);
                T temp = list[i];
                list[i] = list[n];
                list[n] = temp;
            }
        }
    }
}
