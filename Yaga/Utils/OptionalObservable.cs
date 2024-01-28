using System;
using Optional;

namespace Yaga.Utils
{
    public interface IReadOnlyOptionalObservable<T> : IReadOnlyObservable<Option<T>>
    {
        bool HasValue { get; }
        IDisposable Subscribe(Action<T> action, Action onNull);
    }
    
    public interface IOptionalObservable<T> : IReadOnlyOptionalObservable<T>, IObservable<Option<T>>
    {
    }

    public class OptionalObservable<T> : Observable<Option<T>>,IOptionalObservable<T>
    {
        public OptionalObservable(T value) : base(value.Some())
        {
        }

        public bool HasValue => Data.HasValue;
        public IDisposable Subscribe(Action<T> action, Action onNull)
        {
            return Subscribe(data => data.Match(action, onNull));
        }
    }
}