using System;

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
    }
}