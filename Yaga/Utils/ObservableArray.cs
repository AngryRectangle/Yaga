using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Yaga.Utils
{
    public interface IObservableArray<T> : IEnumerable<T>
    {
        event Action<int, T, T> ItemSet;
        T this[int i] { get; set; }
    }

    public class ObservableArray<T> : IObservableArray<T>
    {
        private readonly T[] _array;
        public event Action<int, T, T> ItemSet;

        public T this[int i]
        {
            get => _array[i];
            set
            {
                ItemSet?.Invoke(i, _array[i], value);
                _array[i] = value;
            }
        }

        public ObservableArray(int count)
        {
            _array = new T[count];
        }
        
        public ObservableArray(T[] array)
        {
            _array = array;
        }

        public IEnumerator<T> GetEnumerator() => ((IEnumerable<T>)_array).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _array.GetEnumerator();
    }
}