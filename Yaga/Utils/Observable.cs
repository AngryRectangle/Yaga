using System;

namespace Yaga.Utils
{
    public interface IObservable<T>
    {
        T Data { get; }
        IDisposable Subscribe(Action<T> action);
    }
    /// <summary>
    /// Wrapper around 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Observable<T> : IObservable<T>
    {
        public T Data
        {
            get => _data;
            set
            {
                if(value.Equals(_data))
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
    }
}