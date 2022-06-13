using System.Collections;
using System.Collections.Generic;

namespace Yaga.Utils
{
    public interface IObservableArray<T> : IEnumerable<T>
    {
        Beacon<int, T, T> ItemSet { get; }
        T this[int i] { get; set; }
    }

    public class ObservableArray<T> : IObservableArray<T>
    {
        private readonly T[] _array;
        public Beacon<int, T, T> ItemSet { get; } = new Beacon<int, T, T>();

        public T this[int i]
        {
            get => _array[i];
            set
            {
                ItemSet.Execute(i, _array[i], value);
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