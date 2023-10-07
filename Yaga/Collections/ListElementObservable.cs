using System;
using Yaga.Utils;
using Yaga.Utils.Exceptions;

namespace Yaga.Collections
{
    internal class ListElementObservable<T> : IOptionalObservable<T>
    {
        private readonly ObservableList<T> _list;
        private readonly int _stableIndex;

        public ListElementObservable(ObservableList<T> list, int stableIndex)
        {
            _list = list;
            _stableIndex = stableIndex;
        }

        public T Data => _list.TryGetByStableIndex(_stableIndex, out var result) ? result : throw new EmptyDataAccessException();
        public bool IsDefault => !_list.Has(_stableIndex);

        public IDisposable Subscribe(Action<T> action, Action onNull)
        {
            var setUnsubscription = _list.ItemSet.Add((index, _, to) =>
            {
                if (_list.GetUnstableIndex(_stableIndex) == index)
                    action(to);
            });

            var removeUnsubscription = _list.ItemRemoved.Add((index, lastValue) => onNull());
            return new Reflector(() =>
            {
                setUnsubscription.Dispose();
                removeUnsubscription.Dispose();
            });
        }
    }
}