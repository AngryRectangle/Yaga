using System;

namespace Yaga.Utils
{
    /// <summary>
    /// Class-wrapper that allows to dispose incapsulated actions as if it were methods in classes.
    /// </summary>
    public class Reflector : IDisposable
    {
        private Action _onDispose;

        public Reflector(Action onDispose)
        {
            _onDispose = onDispose;
        }

        public void Dispose() => _onDispose?.Invoke();
    }
}