using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Yaga
{
    public static class SubscriptionsExtension
    {
        /// <summary>
        /// Subscribe on observable value change and dispose subscription when needed.
        /// </summary>
        public static void Subscribe<T>(this ISubscriptions owner, IObservable<T> observable,
            Action<T> onNext)
            => owner.Add(observable.Subscribe(new ActionObserver<T>(onNext, null, null)));

        /// <summary>
        /// Subscribe on observable value change and dispose subscription when needed.
        /// </summary>
        public static void Subscribe<T>(this ISubscriptions owner, IObservable<T> observable,
            Action<T> onNext, Action<Exception> onError)
            => owner.Add(observable.Subscribe(new ActionObserver<T>(onNext, onError, null)));

        /// <summary>
        /// Subscribe on observable value change and dispose subscription when needed.
        /// </summary>
        public static void Subscribe<T>(this ISubscriptions owner, IObservable<T> observable,
            Action<T> onNext, Action<Exception> onError,
            Action onCompleted)
            => owner.Add(observable.Subscribe(new ActionObserver<T>(onNext, onError, onCompleted)));

        /// <summary>
        /// Subscribe on observable value change and dispose subscription when needed.
        /// </summary>
        public static void Subscribe<T>(this ISubscriptions owner, IObservable<T> observable,
            Action<T> onNext, Action onCompleted)
            => owner.Add(observable.Subscribe(new ActionObserver<T>(onNext, null, onCompleted)));

        /// <summary>
        /// Subscribe on UnityEvent and dispose subscription when needed.
        /// </summary>
        public static void Subscribe(this ISubscriptions owner, UnityEvent @event, UnityAction action)
        {
            @event.AddListener(action);
            owner.Add(new Disposable(() => @event.RemoveListener(action)));
        }

        /// <summary>
        /// Subscribe on button click and dispose subscription when needed.
        /// </summary>
        public static void Subscribe(this ISubscriptions owner, Button button, UnityAction action)
        {
            button.onClick.AddListener(action);
            owner.Add(new Disposable(() => button.onClick.RemoveListener(action)));
        }

        /// <summary>
        /// Subscribe on button click, execute action and dispose subscription when needed.
        /// </summary>
        public static void SubscribeAndCall(this ISubscriptions owner, Button button, UnityAction action)
        {
            button.onClick.AddListener(action);
            owner.Add(new Disposable(() => button.onClick.RemoveListener(action)));
            action();
        }

        public static ISubscriptions.Key Set<TView, TModel>(this ISubscriptions owner, TView child, TModel model)
            where TView : IView<TModel>
        {
            var subscriptions = UiBootstrap.Instance.Set(child, model);
            return owner.Add(subscriptions);
        }

        public static ISubscriptions.Key Set<TView>(this ISubscriptions owner, TView child)
            where TView : IView<Unit>
        {
            var subscriptions = UiBootstrap.Instance.Set(child, Unit.Instance);
            return owner.Add(subscriptions);
        }

        public static ViewControl<TChildView, TChildModel> Create<TChildView, TChildModel>(
            this ISubscriptions owner, TChildView childPrefab, TChildModel model, RectTransform parent)
            where TChildView : IView<TChildModel>
        {
            var control = UiControl.Instance.Create(childPrefab, model, parent);
            var key = owner.Add(new Disposable(() => control.Unset()));
            control.Subs.MatchSome(subs => subs.Add(new Disposable(() => owner.Remove(key))));
            return control;
        }

        public static ViewControl<TChildView, TChildModel> Create<TView, TChildView, TChildModel>(
            this ISubscriptions owner, TChildView childPrefab, TChildModel model, TView parent)
            where TView : MonoBehaviour, IView
            where TChildView : IView<TChildModel>
        {
            return Create(owner, childPrefab, model, (RectTransform)parent.transform);
        }

        public static ViewControl<TChildView, Unit> Create<TChildView>(
            this ISubscriptions owner, TChildView childPrefab, RectTransform parent)
            where TChildView : IView<Unit>
        {
            return Create(owner, childPrefab, Unit.Instance, parent);
        }

        public static ViewControl<TChildView, Unit> Create<TView, TChildView>(
            this ISubscriptions owner, TChildView childPrefab, TView parent)
            where TView : MonoBehaviour, IView
            where TChildView : IView<Unit>
        {
            return Create(owner, childPrefab, Unit.Instance, parent);
        }
    }
}