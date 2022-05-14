using System;
using Yaga.Utils;

namespace Yaga.Binding.OptionalObservable
{
    public class ConvertedOptionalObservable<T> : IOptionalObservable<T>
    {
        private readonly Func<T> _dataAccessor;
        private readonly Func<bool> _isDefaultAccessor;

        public ConvertedOptionalObservable(Func<T> dataAccessor, Func<bool> isDefaultAccessor)
        {
            _dataAccessor = dataAccessor;
            _isDefaultAccessor = isDefaultAccessor;
        }

        private event Action<T> OnDataChange;
        private event Action OnDefault;
        public void Perform(T data) => OnDataChange?.Invoke(data);
        public void Default() => OnDefault?.Invoke();

        public T Data => _dataAccessor();
        public bool IsDefault => _isDefaultAccessor();
        public IDisposable Subscribe(Action<T> action, Action onNull)
        {
            OnDataChange += action;
            OnDefault += onNull;
            return new Reflector(() =>
            {
                OnDataChange -= action;
                OnDefault -= onNull;
            });
        }
    }
}