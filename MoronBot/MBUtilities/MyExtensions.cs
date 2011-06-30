using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
    }
}
