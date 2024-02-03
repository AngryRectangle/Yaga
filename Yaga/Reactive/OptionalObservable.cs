using System;
using Optional;

namespace Yaga.Reactive
{
    public interface IReadOnlyOptionalObservable<T> : IReadOnlyObservable<Option<T>>
    {
        bool HasValue { get; }
        IDisposable Subscribe(Action<T> action, Action onNull);
    }

    public interface IOptionalObservable<T> : IReadOnlyOptionalObservable<T>, IObservable<Option<T>>
    {
    }

    public class OptionalObservable<T> : Observable<Option<T>>, IOptionalObservable<T>
    {
        public OptionalObservable(T value) : base(value.Some())
        {
        }

        public bool HasValue => Value.HasValue;

        public IDisposable Subscribe(Action<T> action, Action onNull)
        {
            return Subscribe(data => data.Match(action, onNull));
        }
    }

    internal class OptionalObservable_Where<T> : IReadOnlyOptionalObservable<T>
    {
        private readonly IReadOnlyOptionalObservable<T> _source;
        private readonly Predicate<T> _predicate;

        public OptionalObservable_Where(IReadOnlyOptionalObservable<T> source, Predicate<T> predicate)
        {
            _source = source;
            _predicate = predicate;
        }

        public bool HasValue
        {
            get
            {
                return _source.Value.Match(
                    value => _predicate(value),
                    () => false
                );
            }
        }

        public Option<T> Value => _source.Value.FlatMap(value => _predicate(value) ? value.Some() : Option.None<T>());

        public IDisposable Subscribe(Action<T> action, Action onNull)
        {
            return Subscribe(value => value.Match(action, onNull));
        }

        public IDisposable Subscribe(IObserver<Option<T>> observer)
        {
            return Subscribe(observer.OnNext);
        }

        public IDisposable Subscribe(Action<Option<T>> action)
        {
            return _source.Subscribe(option =>
                option.FlatMap(value => _predicate(value) ? value.Some() : Option.None<T>()));
        }
    }

    internal class OptionalObservable_Select<TIn, TOut> : IReadOnlyOptionalObservable<TOut>
    {
        private readonly IReadOnlyOptionalObservable<TIn> _source;
        private readonly Func<TIn, TOut> _selector;

        public OptionalObservable_Select(IReadOnlyOptionalObservable<TIn> source, Func<TIn, TOut> selector)
        {
            _source = source;
            _selector = selector;
        }

        public Option<TOut> Value => _source.Value.Map(_selector);
        public bool HasValue => _source.HasValue;

        public IDisposable Subscribe(IObserver<Option<TOut>> observer)
        {
            return Subscribe(observer.OnNext);
        }

        public IDisposable Subscribe(Action<Option<TOut>> action)
        {
            return _source.Subscribe(option => action(option.Map(_selector)));
        }

        public IDisposable Subscribe(Action<TOut> action, Action onNull)
        {
            return Subscribe(value => value.Match(action, onNull));
        }
    }

    internal class OptionalObservable_DistinctUntilChanged<T> : IReadOnlyOptionalObservable<T>
    {
        private readonly IReadOnlyOptionalObservable<T> _source;

        public OptionalObservable_DistinctUntilChanged(IReadOnlyOptionalObservable<T> source)
        {
            _source = source;
        }

        public Option<T> Value => _source.Value;
        public bool HasValue => _source.HasValue;

        public IDisposable Subscribe(IObserver<Option<T>> observer)
        {
            return Subscribe(observer.OnNext);
        }

        public IDisposable Subscribe(Action<Option<T>> action)
        {
            var hadValue = false;
            var lastValue = default(Option<T>);
            return _source.Subscribe(option =>
            {
                if (hadValue && lastValue.Equals(option))
                    return;

                lastValue = option;
                hadValue = true;
                action(option);
            });
        }

        public IDisposable Subscribe(Action<T> action, Action onNull)
        {
            return _source.Subscribe(value => value.Match(action, onNull));
        }
    }

    internal class OptionalObservable_CombineLatestWithOptional<T1, T2, TOut> : IReadOnlyOptionalObservable<TOut>
    {
        private readonly IReadOnlyOptionalObservable<T1> _source1;
        private readonly IReadOnlyOptionalObservable<T2> _source2;
        private readonly Func<T1, T2, TOut> _combiner;

        public Option<TOut> Value =>
            _source1.Value.FlatMap(value1 => _source2.Value.Map(value2 => _combiner(value1, value2)));

        public bool HasValue => _source1.HasValue && _source2.HasValue;

        public OptionalObservable_CombineLatestWithOptional(IReadOnlyOptionalObservable<T1> source1,
            IReadOnlyOptionalObservable<T2> source2, Func<T1, T2, TOut> combiner)
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
            var firstSubscription = _source1.Subscribe(value1 =>
                action(value1.FlatMap(value => _source2.Value.Map(value2 => _combiner(value, value2)))));
            var secondSubscription = _source2.Subscribe(value2 =>
                action(value2.FlatMap(value => _source1.Value.Map(value1 => _combiner(value1, value)))));

            return new Disposable(firstSubscription, secondSubscription);
        }

        public IDisposable Subscribe(Action<TOut> action, Action onNull)
        {
            return Subscribe(value => value.Match(action, onNull));
        }
    }
}