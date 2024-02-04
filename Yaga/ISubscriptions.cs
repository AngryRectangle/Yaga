using System;
using System.Collections.Generic;

namespace Yaga
{
    public interface ISubscriptions
    {
        Key Add(IDisposable disposable);
        bool Remove(Key key);
        
        public readonly struct Key : IEqualityComparer<Key>
        {
            private readonly int _key;

            public Key(int key)
            {
                _key = key;
            }

            public bool Equals(Key x, Key y)
            {
                return x._key == y._key;
            }

            public int GetHashCode(Key obj)
            {
                return obj._key;
            }
        }
    }
}