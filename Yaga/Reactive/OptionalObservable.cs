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
}