using System;

namespace Yaga.Utils
{
    public interface IReadOnlyObservable<out T> : System.IObservable<T>
    {
        T Data { get; }
        IDisposable Subscribe(Action<T> action);
    }
    public interface IObservable<T> : IReadOnlyObservable<T>
    {
        T Data { set; get; }
    }

    public static class Observable
    {
        public static Beacon<T2> Bind<T1, T2>(Beacon<T1> observable1, Func<T1, T2> selector)
        {
            var result = new Beacon<T2>();
            observable1.Add(data => result.Execute(selector(data)));
            return result;
        }
        
        public static Beacon<T2> Bind<T1, T2>(IObservable<T1> observable1, Func<T1, T2> selector)
        {
            var result = new Beacon<T2>();
            observable1.Subscribe(data => result.Execute(selector(data)));
            return result;
        }

        public static Beacon<T3> Bind<T1, T2, T3>(
            IObservable<T1> observable1,
            Observable<T2> observable2,
            Func<T1, T2, T3> selector)
        {
            var result = new Beacon<T3>();
            observable1.Subscribe(data => result.Execute(selector(data, observable2.Data)));
            observable2.Subscribe(data => result.Execute(selector(observable1.Data, data)));
            return result;
        }

        public static Beacon<T4> Bind<T1, T2, T3, T4>(
            IObservable<T1> observable1,
            Observable<T2> observable2,
            Observable<T3> observable3,
            Func<T1, T2, T3, T4> selector)
        {
            var result = new Beacon<T4>();
            observable1.Subscribe(data => result.Execute(selector(data, observable2.Data, observable3.Data)));
            observable2.Subscribe(data => result.Execute(selector(observable1.Data, data, observable3.Data)));
            observable3.Subscribe(data => result.Execute(selector(observable1.Data, observable2.Data, data)));
            return result;
        }
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

        public IDisposable Subscribe(IObserver<T> observer)
        {
            return Subscribe(observer.OnNext);
        }
    }
}