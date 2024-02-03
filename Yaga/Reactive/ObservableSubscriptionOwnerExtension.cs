using System;
using Optional;

namespace Yaga.Reactive
{
    public static class ObservableSubscriptionOwnerExtension
    {
        /// <summary>
        /// Subscribe on beacon and dispose subscription when needed.
        /// </summary>
        public static void Subscribe<T1, T2>(this ISubscriptionsOwner owner, Beacon<T1, T2> observable,
            Action<T1, T2> action)
            => owner.Add(observable.Add(action));

        /// <summary>
        /// Subscribe on beacon and dispose subscription when needed.
        /// </summary>
        public static void Subscribe<T1, T2, T3>(this ISubscriptionsOwner owner, Beacon<T1, T2, T3> observable,
            Action<T1, T2, T3> action)
            => owner.Add(observable.Add(action));

        /// <summary>
        /// Subscribe on beacon and dispose subscription when needed.
        /// </summary>
        public static void Subscribe<T>(this ISubscriptionsOwner owner, Beacon<T> observable, Action<T> action)
            => owner.Add(observable.Add(action));

        /// <summary>
        /// Subscribe on observable value change, execute action and dispose subscription when needed.
        /// </summary>
        public static void SubscribeAndCall<T>(this ISubscriptionsOwner owner, IObservable<T> observable,
            Action<T> action)
        {
            owner.Add(observable.Subscribe(action));
            action(observable.Data);
        }

        /// <summary>
        /// Subscribe on observable value change and dispose subscription when needed.
        /// </summary>
        public static void SubscribeAndCall<T>(this ISubscriptionsOwner owner, IOptionalObservable<T> observable,
            Action<T> action, Action onNull)
        {
            owner.Add(observable.Subscribe(action, onNull));
            observable.Data.Match(action, onNull);
        }
        
        /// <summary>
        /// Subscribe on observable value change and dispose subscription when needed.
        /// </summary>
        public static void Subscribe<T>(this ISubscriptionsOwner owner, IOptionalObservable<T> observable,
            Action<T> action, Action onNull)
            => owner.Add(observable.Subscribe(action, onNull));

        /// <summary>
        /// Subscribe on beacon and dispose subscription when needed.
        /// </summary>
        public static void Subscribe(this ISubscriptionsOwner owner, Beacon observable, Action action)
            => owner.Add(observable.Add(action));
        
        /// <summary>
        /// Subscribe on observable value change and dispose subscription when needed.
        /// </summary>
        public static void Subscribe<T>(this ISubscriptionsOwner owner, IObservable<T> observable,
            Action<T> action)
            => owner.Add(observable.Subscribe(action));
        
        public static ISubscriptionsOwner.Key Set<TView, TModel>(this ISubscriptionsOwner owner, TView child,
            IReadOnlyObservable<TModel> observableModel)
            where TView : IView<TModel>
        {
            var cancelKey = Option.None<ISubscriptionsOwner.Key>();
            var unsubscription = observableModel.Subscribe(model =>
            {
                cancelKey.MatchSome(key => owner.Remove(key));
                cancelKey = owner.Set(child, model).Some();
            });

            return owner.Add(unsubscription);
        }
    }
}