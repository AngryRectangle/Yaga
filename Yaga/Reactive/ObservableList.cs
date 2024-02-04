using System.Collections;
using System.Collections.Generic;

namespace Yaga.Reactive
{
    public interface IObservableEnumerable<T> : IEnumerable<T>
    {
        Beacon<int, T> ItemAdded { get; }
        Beacon<int, T> ItemRemoved { get; }
        Beacon<int, T, T> ItemSet { get; }
    }

    /// <summary>
    /// Wrapper around list that allows to track any changes in that list.
    /// </summary>
    public class ObservableList<T> : IObservableEnumerable<T>, IList<T>
    {
        public Beacon<int, T> ItemAdded { get; } = new Beacon<int, T>();
        public Beacon<int, T> ItemRemoved { get; } = new Beacon<int, T>();
        public Beacon<int, T, T> ItemSet { get; } = new Beacon<int, T, T>();
        private List<T> _list;

        public ObservableList()
        {
            _list = new List<T>();
        }

        public ObservableList(IEnumerable<T> enumerable) => _list = new List<T>(enumerable);

        public IEnumerator<T> GetEnumerator() => _list.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Add(T item)
        {
            _list.Add(item);
            ItemAdded.Execute(_list.Count - 1, item);
        }

        public void Clear()
        {
            for (var i = _list.Count - 1; i != 0; i--)
                ItemRemoved.Execute(i, _list[i]);
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
            ItemAdded.Execute(index, item);
        }

        public void RemoveAt(int index)
        {
            _list.RemoveAt(index);
            ItemRemoved.Execute(index, _list[index]);
        }

        public T this[int index]
        {
            get => _list[index];
            set
            {
                ItemSet.Execute(index, _list[index], value);
                _list[index] = value;
            }
        }
    }
}