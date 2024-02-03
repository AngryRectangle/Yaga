using System;

namespace Yaga.Reactive
{
    public static class ObservableExtension
    {
        /// <summary>
        /// Returns an observable that only notifies when the value changes.
        /// </summary>
        public static IReadOnlyObservable<T> DistinctUntilChanged<T>(this IReadOnlyObservable<T> source)
            where T : class
        {
            return new Observable_DistinctUntilChanged<T>(source);
        }

        /// <summary>
        /// Projects each element of an observable sequence into a new form with the specified source and selector.
        /// </summary>
        public static IReadOnlyObservable<TOut> Select<TIn, TOut>(this IReadOnlyObservable<TIn> source,
            Func<TIn, TOut> selector)
        {
            return new Observable_Select<TIn, TOut>(source, selector);
        }

        /// <summary>
        /// If the value satisfies the predicate, returns the value. Otherwise, returns None.
        /// </summary>
        public static IReadOnlyOptionalObservable<T> WhereOrNone<T>(this IReadOnlyObservable<T> source,
            Predicate<T> predicate)
        {
            return new Observable_WhereNone<T>(source, predicate);
        }

        /// <summary>
        /// Returns an observable that combines the latest values of two observables using the specified combiner
        /// and notifies when any of the values changes.
        /// </summary>
        public static IReadOnlyObservable<TOut> CombineLatest<T1, T2, TOut>(this IReadOnlyObservable<T1> source1,
            IReadOnlyObservable<T2> source2, Func<T1, T2, TOut> combiner)
        {
            return new Observable_CombineLatest<T1, T2, TOut>(source1, source2, combiner);
        }
        
        /// <summary>
        /// Returns an observable that combines the latest values of two observables using the specified combiner
        /// and notifies when any of the values changes. If any of the observables has no value, the resulting
        /// observable will also have no value.
        /// </summary>
        public static IReadOnlyOptionalObservable<TOut> CombineLatest<T1, T2, TOut>(
            this IReadOnlyObservable<T1> source1,
            IReadOnlyOptionalObservable<T2> source2, Func<T1, T2, TOut> combiner)
        {
            return new Observable_CombineLatestWithOptional<T1, T2, TOut>(source1, source2, combiner);
        }
    }

    public static class ObservableExtensionsAvoidAmbiguity
    {
        /// <summary>
        /// Returns an observable that only notifies when the value changes.
        /// </summary>
        public static IReadOnlyObservable<T> DistinctUntilChanged<T>(this IReadOnlyObservable<T> source)
            where T : struct, IEquatable<T>
        {
            return new Observable_DistinctUntilChangedEquitable<T>(source);
        }
    }
}