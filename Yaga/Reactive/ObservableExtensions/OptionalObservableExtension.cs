using System;
using Optional;

namespace Yaga.Reactive
{
    public static class OptionalObservableExtension
    {
        /// <summary>
        /// If the value satisfies the predicate and is not none, then the value is passed to the observer.
        /// Otherwise, none is passed to the observer.
        /// </summary>
        public static IReadOnlyOptionalObservable<T> Where<T>(this IReadOnlyOptionalObservable<T> observable,
            Predicate<T> predicate)
        {
            return new OptionalObservable_Where<T>(observable, predicate);
        }

        /// <summary>
        /// Projects each element of an observable sequence into a new form with the specified source and selector.
        /// If the source is none, then the result is none.
        /// </summary>
        public static IReadOnlyOptionalObservable<TOut> Select<TIn, TOut>(
            this IReadOnlyOptionalObservable<TIn> observable,
            Func<TIn, TOut> selector)
        {
            return new OptionalObservable_Select<TIn, TOut>(observable, selector);
        }
        
        /// <summary>
        /// If the value satisfies the predicate, returns the result value from <see cref="TryGet{TIn,TResult}"/>>. Otherwise, returns None.
        /// </summary>
        public static IReadOnlyOptionalObservable<TOut> WhereSelect<TIn, TOut>(this IReadOnlyOptionalObservable<TIn> source, TryGet<TIn, TOut> selector)
        {
            return new Observable_WhereSelect<Option<TIn>, TOut>(source, (Option<TIn> from, out TOut result) =>
            {
                var tempResult = default(TOut);
                var isMatched = from.Match(value => selector(value, out tempResult), () => false);
                result = tempResult;
                return isMatched;
            });
        }

        /// <summary>
        /// Returns an observable that only notifies when the value changes.
        /// </summary>
        public static IReadOnlyOptionalObservable<T> DistinctUntilChanged<T>(this IReadOnlyOptionalObservable<T> source)
            where T : class
        {
            return new OptionalObservable_DistinctUntilChanged<T>(source);
        }

        /// <summary>
        /// Returns an observable that combines the latest values of two observables using the specified combiner
        /// and notifies when any of the values changes. If any of the observables has no value, the resulting
        /// observable will also have no value.
        /// </summary>
        public static IReadOnlyOptionalObservable<TOut> CombineLatest<T1, T2, TOut>(
            this IReadOnlyOptionalObservable<T1> source1,
            IReadOnlyObservable<T2> source2, Func<T1, T2, TOut> combiner)
        {
            return new Observable_CombineLatestWithOptional<T2, T1, TOut>(source2, source1,
                (first, second) => combiner(second, first));
        }

        /// <summary>
        /// Returns an observable that combines the latest values of two observables using the specified combiner
        /// and notifies when any of the values changes. If any of the observables has no value, the resulting
        /// observable will also have no value.
        /// </summary>
        public static IReadOnlyOptionalObservable<TOut> CombineLatest<T1, T2, TOut>(
            this IReadOnlyOptionalObservable<T1> source1,
            IReadOnlyOptionalObservable<T2> source2, Func<T1, T2, TOut> combiner)
        {
            return new OptionalObservable_CombineLatestWithOptional<T1, T2, TOut>(source1, source2, combiner);
        }
    }
}