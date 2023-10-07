using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yaga.Collections;

namespace Yaga.Utils
{
    public interface IObservableEnumerable<T> : IEnumerable<T>
    {
        Beacon<int, T> ItemAdded { get; }
        Beacon<int, T> ItemRemoved { get; }
        Beacon<int, T, T> ItemSet { get; }
    }

    public interface IObservableList<T> : IList<T>, IObservableEnumerable<T>
    {
        IOptionalObservable<T> GetObservable(int index);
    }

    /// <summary>
    /// Wrapper around list that allows to track any changes in that list.
    /// </summary>
    public class ObservableList<T> : IObservableList<T>
    {
        private readonly List<T> _list;
        private readonly List<int> _stableIndexes;
        public Beacon<int, T> ItemAdded { get; } = new Beacon<int, T>();
        public Beacon<int, T> ItemRemoved { get; } = new Beacon<int, T>();
        public Beacon<int, T, T> ItemSet { get; } = new Beacon<int, T, T>();

        public ObservableList()
        {
            _list = new List<T>();
            _stableIndexes = new List<int>();
        }

        public ObservableList(List<T> list)
        {
            _list = list;
            _stableIndexes = new List<int>(_list.Count);
            for (var i = 0; i < _list.Count; i++)
                _stableIndexes.Add(i);
        }

        public ObservableList(IEnumerable<T> enumerable) => _list = new List<T>(enumerable);

        public IEnumerator<T> GetEnumerator() => _list.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Add(T item)
        {
            _stableIndexes.Add(_list.Count);
            _list.Add(item);
            ItemAdded.Execute(_list.Count - 1, item);
        }

        public void Clear()
        {
            for (var i = _list.Count - 1; i >= 0; i--)
                ItemRemoved.Execute(i, _list[i]);
            _list.Clear();
            _stableIndexes.Clear();
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
            for (var i = 0; i < _stableIndexes.Count; i++)
                if (_stableIndexes[i] >= index)
                    _stableIndexes[i]++;

            _stableIndexes.Add(index);
            ItemAdded.Execute(index, item);
        }

        public void RemoveAt(int index)
        {
            var stableIndex = GetStableIndex(index);
            _stableIndexes[stableIndex] = -1;
            for (var i = 0; i < _stableIndexes.Count; i++)
                if (_stableIndexes[i] >= index)
                    _stableIndexes[i]--;

            var item = _list[index];
            _list.RemoveAt(index);
            ItemRemoved.Execute(index, item);
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

        internal bool TryGetByStableIndex(int stableIndex, out T value)
        {
            var unstableIndex = _stableIndexes[stableIndex];
            if (unstableIndex < 0)
            {
                value = default;
                return false;
            }

            value = _list[unstableIndex];
            return true;
        }

        internal bool Has(int stableIndex)
        {
            return _stableIndexes[stableIndex] >= 0;
        }

        internal int GetUnstableIndex(int stableIndex)
        {
            return _stableIndexes[stableIndex];
        }

        public IOptionalObservable<T> GetObservable(int index)
        {
            var stableIndex = GetStableIndex(index);
            Debug.Assert(stableIndex >= 0);
            return new ListElementObservable<T>(this, stableIndex);
        }

        private int GetStableIndex(int index)
        {
            if (index >= _list.Count || index < 0)
                throw new IndexOutOfRangeException();

            var stableIndex = _stableIndexes.IndexOf(index);
            Debug.Assert(stableIndex >= 0);
            return stableIndex;
        }
    }
}