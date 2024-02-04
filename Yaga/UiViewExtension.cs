using System;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Yaga
{
    public static class SubscriptionOwnerExtension
    {
        /// <summary>
        /// Subscribe on observable value change and dispose subscription when needed.
        /// </summary>
        public static void Subscribe<T>(this ISubscriptionsOwner owner, System.IObservable<T> observable,
            Action<T> onNext)
            => owner.Add(observable.Subscribe(new ActionObserver<T>(onNext, null, null)));

        /// <summary>
        /// Subscribe on observable value change and dispose subscription when needed.
        /// </summary>
        public static void Subscribe<T>(this ISubscriptionsOwner owner, System.IObservable<T> observable,
            Action<T> onNext, Action<Exception> onError)
            => owner.Add(observable.Subscribe(new ActionObserver<T>(onNext, onError, null)));

        /// <summary>
        /// Subscribe on observable value change and dispose subscription when needed.
        /// </summary>
        public static void Subscribe<T>(this ISubscriptionsOwner owner, System.IObservable<T> observable,
            Action<T> onNext, Action<Exception> onError,
            Action onCompleted)
            => owner.Add(observable.Subscribe(new ActionObserver<T>(onNext, onError, onCompleted)));

        /// <summary>
        /// Subscribe on observable value change and dispose subscription when needed.
        /// </summary>
        public static void Subscribe<T>(this ISubscriptionsOwner owner, System.IObservable<T> observable,
            Action<T> onNext, Action onCompleted)
            => owner.Add(observable.Subscribe(new ActionObserver<T>(onNext, null, onCompleted)));

        /// <summary>
        /// Subscribe on UnityEvent and dispose subscription when needed.
        /// </summary>
        public static void Subscribe(this ISubscriptionsOwner owner, UnityEvent @event, UnityAction action)
        {
            @event.AddListener(action);
            owner.Add(new Disposable(() => @event.RemoveListener(action)));
        }

        /// <summary>
        /// Subscribe on button click and dispose subscription when needed.
        /// </summary>
        public static void Subscribe(this ISubscriptionsOwner owner, Button button, UnityAction action)
        {
            button.onClick.AddListener(action);
            owner.Add(new Disposable(() => button.onClick.RemoveListener(action)));
        }

        /// <summary>
        /// Subscribe on button click, execute action and dispose subscription when needed.
        /// </summary>
        public static void SubscribeAndCall(this ISubscriptionsOwner owner, Button button, UnityAction action)
        {
            button.onClick.AddListener(action);
            owner.Add(new Disposable(() => button.onClick.RemoveListener(action)));
            action();
        }

        public static ISubscriptionsOwner.Key Set<TView, TModel>(this ISubscriptionsOwner owner, TView child, TModel model)
            where TView : IView<TModel>
        {
            var subscriptions = UiBootstrap.Instance.Set(child, model);
            return owner.Add(subscriptions);
        }

        public static ISubscriptionsOwner.Key Set<TView>(this ISubscriptionsOwner owner, TView child)
            where TView : IView<Unit>
        {
            var subscriptions = UiBootstrap.Instance.Set(child, Unit.Instance);
            return owner.Add(subscriptions);
        }
    }
}