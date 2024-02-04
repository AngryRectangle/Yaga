using System;
using System.Collections.Generic;
using System.Linq;

namespace Yaga
{
    internal class Subscriptions : ISubscriptions, IDisposable
    {
        // 1 is initial index to prevent treating 0 key as real key which can be created as default value for Key struct.
        private int _keyIndex = 1;

        private readonly Dictionary<ISubscriptions.Key, IDisposable> _disposables =
            new Dictionary<ISubscriptions.Key, IDisposable>();

        public ISubscriptions.Key Add(IDisposable disposable)
        {
            var key = new ISubscriptions.Key(_keyIndex++);
            _disposables.Add(key, disposable);
            return key;
        }

        public bool Remove(ISubscriptions.Key key)
        {
            return _disposables.Remove(key);
        }

        public void Dispose()
        {
            foreach (var pair in _disposables.ToArray())
                pair.Value.Dispose();

            _disposables.Clear();
        }
    }
}