using System;

namespace Yaga
{
    internal class ReplacableDisposable : IDisposable
    {
        private IDisposable _disposable;

        public void Replace(IDisposable disposable)
        {
            _disposable = disposable;
        }

        public void Remove()
        {
            _disposable = null;
        }

        public void Dispose()
        {
            _disposable?.Dispose();
        }
    }
}