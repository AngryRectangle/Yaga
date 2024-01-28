using System;

namespace Yaga.Utils
{
    /// <summary>
    /// Class-wrapper that allows to dispose incapsulated actions as if it were methods in classes.
    /// </summary>
    public class Disposable : IDisposable
    {
        private readonly Action _onDispose;
        private bool _disposed;

        public Disposable(Action onDispose)
        {
            _onDispose = onDispose;
        }
        
        public Disposable(params IDisposable[] disposable)
        {
            _onDispose = () =>
            {
                foreach (var d in disposable)
                    d.Dispose();
            };
        }

        public void Dispose()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(Disposable));
            
            _disposed = true;
            _onDispose?.Invoke();
        }
    }
}