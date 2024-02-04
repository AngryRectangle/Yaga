using System;
using Optional;

namespace Yaga.Reactive
{
    public static class ObservableSubscriptionOwnerExtension
    {
        /// <summary>
        /// Subscribe on observable value change, execute action and dispose subscription when needed.
        /// </summary>
        public static void SubscribeAndCall<T>(this ISubscriptions owner, IObservable<T> observable,
            Action<T> action)
        {
            owner.Add(observable.Subscribe(action));
            action(observable.Value);
        }

        /// <summary>
        /// Subscribe on observable value change and dispose subscription when needed.
        /// </summary>
        public static void SubscribeAndCall<T>(this ISubscriptions owner, IOptionalObservable<T> observable,
            Action<T> action, Action onNull)
        {
            owner.Add(observable.Subscribe(action, onNull));
            observable.Value.Match(action, onNull);
        }

        /// <summary>
        /// Subscribe on observable value change and dispose subscription when needed.
        /// </summary>
        public static void Subscribe<T>(this ISubscriptions owner, IOptionalObservable<T> observable,
            Action<T> action, Action onNull)
            => owner.Add(observable.Subscribe(action, onNull));

        /// <summary>
        /// Subscribe on observable value change and dispose subscription when needed.
        /// </summary>
        public static void Subscribe<T>(this ISubscriptions owner, IObservable<T> observable,
            Action<T> action)
            => owner.Add(observable.Subscribe(action));

        public static ISubscriptions.Key Set<TView, TModel>(this ISubscriptions owner, TView child,
            IReadOnlyObservable<TModel> observableModel)
            where TView : IView<TModel>
        {
            var cancelKey = Option.None<ISubscriptions.Key>();
            var unsubscription = observableModel.Subscribe(model =>
            {
                cancelKey.MatchSome(key => owner.Remove(key));
                var viewControl = owner.Set(child, model);
                cancelKey = owner.Add(new Disposable(() => viewControl.Unset())).Some();
            });

            return owner.Add(unsubscription);
        }
    }
}