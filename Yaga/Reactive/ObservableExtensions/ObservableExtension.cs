using System;
using Optional;

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

        public static IReadOnlyObservable<Option<TOut>> SelectMap<TIn, TOut>(
            this IReadOnlyObservable<Option<TIn>> source,
            Func<TIn, TOut> selector)
        {
            return new Observable_Select<Option<TIn>, Option<TOut>>(source, value => value.Map(selector));
        }

        public static IReadOnlyObservable<TOut> SelectMatch<TIn, TOut>(this IReadOnlyObservable<Option<TIn>> source,
            Func<TIn, TOut> selector, Func<TOut> onNoneSelector)
        {
            return new Observable_Select<Option<TIn>, TOut>(source, value => value.Match(selector, onNoneSelector));
        }

        public static IReadOnlyObservable<TOut> SelectMatch<TIn, TOut>(this IReadOnlyObservable<Option<TIn>> source,
            Func<TIn, TOut> selector, TOut onNoneValue)
        {
            return new Observable_Select<Option<TIn>, TOut>(source, value => value.Match(selector, () => onNoneValue));
        }
        
        public static IReadOnlyObservable<bool> SelectIfSome<TIn>(this IReadOnlyObservable<Option<TIn>> source,
            Func<TIn, bool> selector)
        {
            return new Observable_Select<Option<TIn>, bool>(source, value => value.Match(selector, () => false));
        }

        /// <summary>
        /// If the value satisfies the predicate, returns the result value from <see cref="TryGet{TIn,TResult}"/>>. Otherwise, returns None.
        /// </summary>
        public static IReadOnlyOptionalObservable<TOut> WhereSelect<TIn, TOut>(this IReadOnlyObservable<TIn> source,
            TryGet<TIn, TOut> selector)
        {
            return new Observable_WhereSelect<TIn, TOut>(source, selector);
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

        public static IReadOnlyObservable<TOut> Unfold<TIn, TOut>(this IReadOnlyObservable<TIn> source)
            where TIn : IReadOnlyObservable<TOut>
        {
            return new Observable_Unfold<TOut, TIn>(source);
        }

        public static IReadOnlyObservable<Option<TOut>> UnfoldOption<TIn, TOut>(
            this IReadOnlyObservable<Option<TIn>> source)
            where TIn : IReadOnlyObservable<TOut>
        {
            return new Observable_OptionUnfold<TOut, TIn>(source);
        }

        public static IReadOnlyObservable<Option<TOut>> UnfoldOption<TOut>(
            this IReadOnlyObservable<Option<IReadOnlyObservable<TOut>>> source)
        {
            return new Observable_OptionUnfold<TOut, IReadOnlyObservable<TOut>>(source);
        }

        public static IReadOnlyObservable<Option<TOut>> UnfoldOption<TOut>(
            this IReadOnlyObservable<Option<Observable<TOut>>> source)
        {
            return new Observable_OptionUnfold<TOut, Observable<TOut>>(source);
        }

        public static void SetNone<T>(this IObservable<Option<T>> observable)
        {
            observable.Value = Option.None<T>();
        }

        public static void SetValue<T>(this IObservable<Option<T>> observable, T value)
        {
            observable.Value = value.Some();
        }

        public static IDisposable Is<T>(this IObservable<T> observable, IReadOnlyObservable<T> fromObservable)
        {
            observable.Value = fromObservable.Value;
            return fromObservable.Subscribe(value => { observable.Value = value; });
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