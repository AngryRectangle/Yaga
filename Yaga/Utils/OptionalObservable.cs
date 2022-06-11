using System;

namespace Yaga.Utils
{
    public interface IOptionalObservable<T>
    {
        T Data { get; }
        bool IsDefault { get; }
        IDisposable Subscribe(Action<T> action, Action onNull);
    }

    public static class OptionalObservable
    {
        public static BoundOptionalObservable<T2> Bind<T1, T2>(
            IOptionalObservable<T1> observable1,
            Func<T1, T2> selector)
        {
            var result = new BoundOptionalObservable<T2>();
            observable1.Subscribe(data => result.Data = selector(data), result.SetDefault);
            return result;
        }

        public static BoundOptionalObservable<T3> Bind<T1, T2, T3>(
            IOptionalObservable<T1> observable1,
            IOptionalObservable<T2> observable2,
            Func<T1, T2, T3> selector)
        {
            var result = new BoundOptionalObservable<T3>();
            observable1.Subscribe(data =>
            {
                if (!observable2.IsDefault)
                    result.Data = selector(data, observable2.Data);
            }, result.SetDefault);

            observable2.Subscribe(data =>
            {
                if (!observable1.IsDefault)
                    result.Data = selector(observable1.Data, data);
            }, result.SetDefault);

            return result;
        }

        public static BoundOptionalObservable<T3> Bind<T1, T2, T3>(
            IObservable<T1> observable1,
            IOptionalObservable<T2> observable2,
            Func<T1, T2, T3> selector)
        {
            var result = new BoundOptionalObservable<T3>();
            observable1.Subscribe(data =>
            {
                if (!observable2.IsDefault)
                    result.Data = selector(data, observable2.Data);
            });

            observable2.Subscribe(data => result.Data = selector(observable1.Data, data), result.SetDefault);

            return result;
        }
    }

    public class OptionalObservable<T> : IOptionalObservable<T>
    {
        public OptionalObservable()
        {
            IsDefault = true;
        }

        public OptionalObservable(T value)
        {
            _data = value;
        }

        private event Action<T> OnChange;
        private event Action OnNull;

        private T _data;

        public T Data
        {
            get => _data;
            set
            {
                if (value.Equals(_data) && !IsDefault)
                    return;

                IsDefault = false;
                OnChange?.Invoke(value);
                _data = value;
            }
        }

        public void SetDefault()
        {
            _data = default;
            IsDefault = true;
            OnNull?.Invoke();
        }

        public bool IsDefault { private set; get; }

        public IDisposable Subscribe(Action<T> action, Action onNull)
        {
            OnChange += action;
            OnNull += onNull;
            return new Reflector(() =>
            {
                OnChange -= action;
                OnNull -= onNull;
            });
        }
    }

    public class BoundOptionalObservable<T> : IOptionalObservable<T>
    {
        private event Action<T> OnChange;
        private event Action OnNull;

        private T _data;

        public T Data
        {
            get => _data;
            internal set
            {
                if (value.Equals(_data) && !IsDefault)
                    return;

                IsDefault = false;
                OnChange?.Invoke(value);
                _data = value;
            }
        }

        public bool IsDefault { private set; get; }

        internal void SetDefault()
        {
            _data = default;
            IsDefault = true;
            OnNull?.Invoke();
        }

        public IDisposable Subscribe(Action<T> action, Action onNull)
        {
            OnChange += action;
            OnNull += onNull;
            return new Reflector(() =>
            {
                OnChange -= action;
                OnNull -= onNull;
            });
        }
    }
}