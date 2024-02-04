using System;

namespace Yaga.Reactive.Binding.Observable
{
    internal class ConvertedObservable<T> : IReadOnlyObservable<T>
    {
        private readonly Func<T> _dataAccessor;

        public ConvertedObservable(Func<T> dataAccessor)
        {
            _dataAccessor = dataAccessor;
        }

        private event Action<T> OnDataChange;
        public void Perform(T data) => OnDataChange?.Invoke(data);

        public T Value => _dataAccessor();

        public IDisposable Subscribe(Action<T> action)
        {
            OnDataChange += action;
            return new Disposable(() => OnDataChange -= action);
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            return Subscribe(observer.OnNext);
        }
    }
}