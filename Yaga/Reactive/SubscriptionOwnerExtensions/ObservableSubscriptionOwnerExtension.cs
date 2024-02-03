using System;
using Optional;

namespace Yaga.Reactive
{
    public static class ObservableSubscriptionOwnerExtension
    {
        /// <summary>
        /// Subscribe on observable value change, execute action and dispose subscription when needed.
        /// </summary>
        public static void SubscribeAndCall<T>(this ISubscriptionsOwner owner, IObservable<T> observable,
            Action<T> action)
        {
            owner.Add(observable.Subscribe(action));
            action(observable.Value);
        }

        /// <summary>
        /// Subscribe on observable value change and dispose subscription when needed.
        /// </summary>
        public static void SubscribeAndCall<T>(this ISubscriptionsOwner owner, IOptionalObservable<T> observable,
            Action<T> action, Action onNull)
        {
            owner.Add(observable.Subscribe(action, onNull));
            observable.Value.Match(action, onNull);
        }
        
        /// <summary>
        /// Subscribe on observable value change and dispose subscription when needed.
        /// </summary>
        public static void Subscribe<T>(this ISubscriptionsOwner owner, IOptionalObservable<T> observable,
            Action<T> action, Action onNull)
            => owner.Add(observable.Subscribe(action, onNull));
        
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