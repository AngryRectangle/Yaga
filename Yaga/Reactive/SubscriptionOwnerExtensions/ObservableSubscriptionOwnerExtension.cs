using System;
using Optional;
using UnityEngine;

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

        public static IDisposable Set<TView, TModel>(this ISubscriptions owner, TView child,
            IReadOnlyObservable<TModel> observableModel)
            where TView : IView<TModel>
        {
            var viewControl = owner.Set(child, observableModel.Value);
            var cancelKey = owner.Add(new Disposable(() => viewControl.Unset()));
            var unsubscription = observableModel.Subscribe(model =>
            {
                var result = owner.Remove(cancelKey);
                Debug.Assert(result, "Key is stored only here, so it should be removed successfully");
                viewControl = owner.Set(child, model);
                cancelKey = owner.Add(new Disposable(() => viewControl.Unset()));
            });

            var key =  owner.Add(unsubscription);
            return new Disposable(() =>
            {
                owner.Remove(key);
                unsubscription.Dispose();
            });
        }
    }
}