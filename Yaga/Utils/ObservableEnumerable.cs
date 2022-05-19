using System;
using System.Collections;
using System.Collections.Generic;

namespace Yaga.Utils
{
    public interface IObservableEnumerable<out T> : IEnumerable<T>
    {
        event Action<int, T> ItemAdded;
        event Action<int, T> ItemRemoved;
        event Action<int, T, T> ItemSet;
    }

    /// <summary>
    /// Wrapper around list that allows to track any changes in that list.
    /// </summary>
    public class ObservableEnumerable<T> : IObservableEnumerable<T>, IList<T>
    {
        public event Action<int, T> ItemAdded;
        public event Action<int, T> ItemRemoved;
        public event Action<int, T, T> ItemSet;
        private List<T> _list;

        public ObservableEnumerable()
        {
            _list = new List<T>();
        }

        public ObservableEnumerable(IEnumerable<T> enumerable) => _list = new List<T>(enumerable);

        public IEnumerator<T> GetEnumerator() => _list.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Add(T item)
        {
            _list.Add(item);
            ItemAdded?.Invoke(_list.Count - 1, item);
        }

        public void Clear()
        {
            for (var i = _list.Count - 1; i != 0; i--)
                ItemRemoved?.Invoke(i, _list[i]);
            _list.Clear();
        }

        public bool Contains(T item) => _list.Contains(item);

        public void CopyTo(T[] array, int arrayIndex) => _list.CopyTo(array, arrayIndex);

        public bool Remove(T item)
        {
            var index = IndexOf(item);
            if (index < 0)
                return false;
            RemoveAt(index);
            return true;
        }

        public int Count => _list.Count;
        public bool IsReadOnly => false;
        public int IndexOf(T item) => _list.IndexOf(item);

        public void Insert(int index, T item)
        {
            _list.Insert(index, item);
            ItemAdded?.Invoke(index, item);
        }

        public void RemoveAt(int index)
        {
            _list.RemoveAt(index);
            ItemRemoved?.Invoke(index, _list[index]);
        }

        public T this[int index]
        {
            get => _list[index];
            set
            {
                ItemSet?.Invoke(index, _list[index], value);
                _list[index] = value;
            }
        }
    }
}