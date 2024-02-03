using System;
using System.Collections.Generic;

namespace Yaga
{
    public class Subscriptions : ISubscriptionsOwner, IDisposable
    {
        // 1 is initial index to prevent treating 0 key as real key which can be created as default value for Key struct.
        private int _keyIndex = 1;

        private readonly Dictionary<ISubscriptionsOwner.Key, IDisposable> _disposables =
            new Dictionary<ISubscriptionsOwner.Key, IDisposable>();

        public ISubscriptionsOwner.Key Add(IDisposable disposable)
        {
            var key = new ISubscriptionsOwner.Key(_keyIndex++);
            _disposables.Add(key, disposable);
            return key;
        }

        public bool Remove(ISubscriptionsOwner.Key key)
        {
            return _disposables.Remove(key);
        }

        public void Dispose()
        {
            foreach (var pair in _disposables)
                pair.Value.Dispose();

            _disposables.Clear();
        }
    }
}