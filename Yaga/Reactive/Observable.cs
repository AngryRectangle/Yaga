using System;
using System.Collections.Generic;
using Optional;

namespace Yaga.Reactive
{
    public interface IReadOnlyObservable<out T> : System.IObservable<T>
    {
        T Value { get; }
        IDisposable Subscribe(Action<T> action);
    }

    public interface IObservable<T> : IReadOnlyObservable<T>
    {
        T Value { set; get; }
    }

    /// <summary>
    /// Provider of notification information with disposable subscription.
    /// </summary>
    public class Observable<T> : IObservable<T>
    {
        private event Action<T> OnChange;
        private T _data;

        public T Value
        {
            get => _data;
            set
            {
                _data = value;
                OnChange?.Invoke(_data);
            }
        }

        public Observable(T data)
        {
            _data = data;
        }

        public Observable()
        {
        }

        public IDisposable Subscribe(Action<T> action)
        {
            OnChange += action;
            return new Disposable(() => OnChange -= action);
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            return Subscribe(observer.OnNext);
        }
    }

    internal class Observable_DistinctUntilChanged<T> : IReadOnlyObservable<T>
        where T : class
    {
        private readonly IReadOnlyObservable<T> _source;

        public Observable_DistinctUntilChanged(IReadOnlyObservable<T> source)
        {
            _source = source;
        }

        public T Value => _source.Value;

        public IDisposable Subscribe(IObserver<T> observer)
        {
            return Subscribe(observer.OnNext);
        }

        public IDisposable Subscribe(Action<T> action)
        {
            var lastValue = default(T);
            var hadValue = false;
            return _source.Subscribe(value =>
            {
                if (hadValue && lastValue.Equals(value))
                    return;

                lastValue = value;
                hadValue = true;
                action(value);
            });
        }
    }

    internal class Observable_DistinctUntilChangedEquitable<T> : IReadOnlyObservable<T>
        where T : struct, IEquatable<T>
    {
        private readonly IReadOnlyObservable<T> _source;

        public Observable_DistinctUntilChangedEquitable(IReadOnlyObservable<T> source)
        {
            _source = source;
        }

        public T Value => _source.Value;

        public IDisposable Subscribe(IObserver<T> observer)
        {
            return Subscribe(observer.OnNext);
        }

        public IDisposable Subscribe(Action<T> action)
        {
            var lastValue = default(T);
            var hadValue = false;
            return _source.Subscribe(value =>
            {
                if (hadValue && lastValue.Equals(value))
                    return;

                lastValue = value;
                hadValue = true;
                action(value);
            });
        }
    }

    internal class Observable_Select<TIn, TOut> : IReadOnlyObservable<TOut>
    {
        private readonly IReadOnlyObservable<TIn> _source;
        private readonly Func<TIn, TOut> _selector;

        public Observable_Select(IReadOnlyObservable<TIn> source, Func<TIn, TOut> selector)
        {
            _source = source;
            _selector = selector;
        }

        public TOut Value => _selector(_source.Value);

        public IDisposable Subscribe(IObserver<TOut> observer)
        {
            return Subscribe(observer.OnNext);
        }

        public IDisposable Subscribe(Action<TOut> action)
        {
            return _source.Subscribe(value => action(_selector(value)));
        }
    }

    internal class Observable_WhereNone<T> : IReadOnlyOptionalObservable<T>
    {
        private readonly IReadOnlyObservable<T> _source;
        private readonly Predicate<T> _predicate;

        public Option<T> Value => _predicate(_source.Value) ? _source.Value.Some() : Option.None<T>();
        public bool HasValue => _predicate(_source.Value);

        public Observable_WhereNone(IReadOnlyObservable<T> source, Predicate<T> predicate)
        {
            _source = source;
            _predicate = predicate;
        }

        public IDisposable Subscribe(IObserver<Option<T>> observer)
        {
            return Subscribe(observer.OnNext);
        }

        public IDisposable Subscribe(Action<Option<T>> action)
        {
            return _source.Subscribe(value => action(_predicate(value) ? value.Some() : Option.None<T>()));
        }

        public IDisposable Subscribe(Action<T> action, Action onNull)
        {
            return _source.Subscribe(value =>
            {
                if (_predicate(value))
                    action(value);
                else
                    onNull();
            });
        }
    }
    public delegate bool TryGet<in TIn, TResult>(TIn from, out TResult to);
    internal class Observable_WhereSelect<TIn, TOut> : IReadOnlyOptionalObservable<TOut>
    {
        private readonly IReadOnlyObservable<TIn> _source;
        private readonly TryGet<TIn, TOut> _predicate;

        public Option<TOut> Value => _predicate(_source.Value, out var result) ? result.Some() : Option.None<TOut>();
        public bool HasValue => _predicate(_source.Value, out _);

        public Observable_WhereSelect(IReadOnlyObservable<TIn> source, TryGet<TIn, TOut> predicate)
        {
            _source = source;
            _predicate = predicate;
        }

        public IDisposable Subscribe(IObserver<Option<TOut>> observer)
        {
            return Subscribe(observer.OnNext);
        }

        public IDisposable Subscribe(Action<Option<TOut>> action)
        {
            return _source.Subscribe(value => action(_predicate(value, out var result) ? result.Some() : Option.None<TOut>()));
        }

        public IDisposable Subscribe(Action<TOut> action, Action onNull)
        {
            return _source.Subscribe(value =>
            {
                if (_predicate(value, out var result))
                    action(result);
                else
                    onNull();
            });
        }
    }

    internal class Observable_CombineLatest<T1, T2, TOut> : IReadOnlyObservable<TOut>
    {
        private readonly IReadOnlyObservable<T1> _source1;
        private readonly IReadOnlyObservable<T2> _source2;
        private readonly Func<T1, T2, TOut> _combiner;

        public TOut Value => _combiner(_source1.Value, _source2.Value);

        public Observable_CombineLatest(IReadOnlyObservable<T1> source1, IReadOnlyObservable<T2> source2,
            Func<T1, T2, TOut> combiner)
        {
            _source1 = source1;
            _source2 = source2;
            _combiner = combiner;
        }

        public IDisposable Subscribe(IObserver<TOut> observer)
        {
            return Subscribe(observer.OnNext);
        }

        public IDisposable Subscribe(Action<TOut> action)
        {
            return _source1.Subscribe(value1 => action(_combiner(value1, _source2.Value)));
        }
    }

    internal class Observable_CombineLatestWithOptional<T1, T2, TOut> : IReadOnlyOptionalObservable<TOut>
    {
        private readonly IReadOnlyObservable<T1> _source1;
        private readonly IReadOnlyObservable<Option<T2>> _source2;
        private readonly Func<T1, T2, TOut> _combiner;
        public Option<TOut> Value => _source2.Value.Map(value2 => _combiner(_source1.Value, value2));
        public bool HasValue => _source2.Value.HasValue;

        public Observable_CombineLatestWithOptional(IReadOnlyObservable<T1> source1,
            IReadOnlyObservable<Option<T2>> source2, Func<T1, T2, TOut> combiner)
        {
            _source1 = source1;
            _source2 = source2;
            _combiner = combiner;
        }

        public IDisposable Subscribe(IObserver<Option<TOut>> observer)
        {
            return Subscribe(observer.OnNext);
        }

        public IDisposable Subscribe(Action<Option<TOut>> action)
        {
            var firstUnsubscription = _source1.Subscribe(value1 =>
                action(_source2.Value.Map(value2 => _combiner(value1, value2))));

            var secondUnsubscription = _source2.Subscribe(option =>
                action(option.Map(value2 => _combiner(_source1.Value, value2))));

            return new Disposable(firstUnsubscription, secondUnsubscription);
        }

        public IDisposable Subscribe(Action<TOut> action, Action onNull)
        {
            return Subscribe(option => option.Match(action, onNull));
        }
    }
}