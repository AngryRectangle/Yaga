using System;

namespace Yaga.Utils
{
    public interface IObservable<T>
    {
        T Data { get; }
        IDisposable Subscribe(Action<T> action);
    }

    /// <summary>
    /// Provider of notification information with disposable subscription.
    /// </summary>
    public class Observable<T> : IObservable<T>
    {
        public T Data
        {
            get => _data;
            set
            {
                if (value.Equals(_data))
                    return;

                OnChange?.Invoke(value);
                _data = value;
            }
        }

        private T _data;

        public Observable(T data)
        {
            _data = data;
        }

        public Observable()
        {
        }

        private event Action<T> OnChange;

        public IDisposable Subscribe(Action<T> action)
        {
            OnChange += action;
            return new Reflector(() => OnChange -= action);
        }
        
        public static IDisposable operator +(Observable<T> a, Action<T> action) => a.Subscribe(action);

        public IDisposable Bind<T1>(IObservable<T1> observable1, Func<T1, T> selector)
            => observable1.Subscribe(data => Data = selector(data));

        public IDisposable Bind<T1, T2>(
            IObservable<T1> observable1,
            IObservable<T2> observable2,
            Func<T1, T2, T> selector)
        {
            var firstDisposer = observable1.Subscribe(data => Data = selector(data, observable2.Data));
            var secondDisposer = observable2.Subscribe(data => Data = selector(observable1.Data, data));
            return new Reflector(() =>
            {
                firstDisposer.Dispose();
                secondDisposer.Dispose();
            });
        }
        
        public IDisposable Bind<T1, T2, T3>(
            IObservable<T1> observable1,
            IObservable<T2> observable2,
            IObservable<T3> observable3,
            Func<T1, T2, T3, T> selector)
        {
            var firstDisposer = observable1.Subscribe(data => Data = selector(data, observable2.Data, observable3.Data));
            var secondDisposer = observable2.Subscribe(data => Data = selector(observable1.Data, data, observable3.Data));
            var thirdDisposer = observable3.Subscribe(data => Data = selector(observable1.Data, observable2.Data, data));
            return new Reflector(() =>
            {
                firstDisposer.Dispose();
                secondDisposer.Dispose();
                thirdDisposer.Dispose();
            });
        }
    }
}