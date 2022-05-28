﻿using System;

namespace Yaga.Utils
{
    public interface IOptionalObservable<T>
    {
        T Data { get; }
        bool IsDefault { get; }
        IDisposable Subscribe(Action<T> action, Action onNull);
    }

    public class OptionalObservable<T> : IOptionalObservable<T>
    {
        public OptionalObservable()
        {
            IsDefault = true;
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
        
        public IDisposable Bind<T1>(
            IOptionalObservable<T1> observable1,
            Func<T1, T> selector
        )
        {
            return observable1.Subscribe(data =>
            {
                Data = selector(data);
            }, SetDefault);
        }

        public IDisposable Bind<T1, T2>(
            IOptionalObservable<T1> observable1,
            IOptionalObservable<T2> observable2,
            Func<T1, T2, T> selector
        )
        {
            var firstDisposer = observable1.Subscribe(data =>
            {
                if (observable2.IsDefault)
                    return;
                Data = selector(data, observable2.Data);
            }, SetDefault);

            var secondDisposer= observable2.Subscribe(data =>
            {
                if (observable1.IsDefault)
                    return;
                Data = selector(observable1.Data, data);
            }, SetDefault);

            return new Reflector(() =>
            {
                firstDisposer.Dispose();
                secondDisposer.Dispose();
            });
        }
        
        public IDisposable Bind<T1, T2>(
            IObservable<T1> observable1,
            IOptionalObservable<T2> observable2,
            Func<T1, T2, T> selector
        )
        {
            var firstDisposer = observable1.Subscribe(data =>
            {
                if (observable2.IsDefault)
                    return;
                Data = selector(data, observable2.Data);
            });

            var secondDisposer= observable2.Subscribe(data =>
            {
                Data = selector(observable1.Data, data);
            }, SetDefault);

            return new Reflector(() =>
            {
                firstDisposer.Dispose();
                secondDisposer.Dispose();
            });
        }
    }
}