using System;
using System.Collections.Generic;

namespace Yaga
{
    public static class ListExtension
    {
        public static bool TryRemove<T>(this List<T> data, T value)
        where T: IEquatable<T>
        {
            for (var i = 0; i < data.Count; i++)
            {
                if(!data[i].Equals(value))
                    continue;
                
                data.RemoveAt(i);
                return true;
            }

            return false;
        }
    }
}