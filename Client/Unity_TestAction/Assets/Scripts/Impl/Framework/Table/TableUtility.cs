using System;
using System.Collections.Generic;
using System.Text;

namespace JW_Table
{
    public static class TableUtility
    {
        public static Int64 Combine(Int64 k1, Int64 k2, int l)
        {
            return (k1 << (l * 8)) + k2;
        }

        public static Int64 Combine(Int64 k1, Int64 k2, int l1, Int64 k3, int l2)
        {
            return Combine(Combine(k1, k2, l1), k3, l2);
        }

        public static int BinarySearch<T>(List<T> list, Int64 key)
        {
            int start = 0, end = list.Count - 1, middle;

            while (start <= end)
            {
                middle = (start + end) >> 1;
                T item = list[middle];
                IKey kb = item as IKey;
                if (kb.Key() == key)
                    return middle;
                else if (key < kb.Key())
                    end = middle - 1;
                else
                    start = middle + 1;
            }

            return -1;
        }
    }
}
