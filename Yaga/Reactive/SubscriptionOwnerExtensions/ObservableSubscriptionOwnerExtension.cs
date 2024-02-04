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

        /// <summary>
        /// Set view with model and subscribe on model changes.
        /// If value in observable changes, view model will be updated.
        /// If view is unset or destroyed, subscription will be disposed.
        /// If parent view is unset or destroyed, child view will be destroyed too.
        /// Also it is possible to dispose subscription manually by using return value.
        /// </summary>
        /// <returns>Manual disposable that allows to end subscription on call site</returns>
        public static IDisposable Set<TView, TModel>(this ISubscriptions owner, TView child,
            IReadOnlyObservable<TModel> observableModel)
            where TView : IView<TModel>
        {
            ISubscriptions.Key viewCancelKey = default;
            var unsubscription = new ReplacableDisposable();

            var viewControl = owner.Set(child, observableModel.Value);
            viewControl.Subs.MatchSome(subs => viewCancelKey = subs.Add(unsubscription));
            Debug.Assert(viewControl.Subs.HasValue);

            var cancelKey = owner.Add(new Disposable(() => viewControl.Unset()));
            var tempDisposable = observableModel.Subscribe(model =>
            {
                var result = owner.Remove(cancelKey) &&
                             viewControl.Subs.Match(subs => subs.Remove(viewCancelKey), () => false);

                Debug.Assert(result, "Key is stored only here, so it should be removed successfully");
                viewControl = owner.Set(child, model);
                cancelKey = owner.Add(new Disposable(() => viewControl.Unset()));
                viewControl.Subs.MatchSome(subs => viewCancelKey = subs.Add(unsubscription));
                Debug.Assert(viewControl.Subs.HasValue);
            });

            var key = owner.Add(unsubscription);
            unsubscription.Replace(new Disposable(() =>
            {
                owner.Remove(key);
                viewControl.Subs.MatchSome(subs => subs.Remove(viewCancelKey));
                Debug.Assert(viewControl.Subs.HasValue);
                tempDisposable.Dispose();
                unsubscription.Replace(null);
            }));

            return unsubscription;
        }

        internal static IDisposable Set<TView, TModel>(this ISubscriptions owner, TView child,
            IReadOnlyObservable<Option<TModel>> observableModel, OptionalStrategy strategy = OptionalStrategy.Activity)
            where TView : MonoBehaviour, IView<TModel>
        {
            Option<(ViewControl<TView, TModel> viewControl, ISubscriptions.Key key)> ViewControl(Option<TModel> option)
            {
                return option.Match(value =>
                {
                    var viewControl = owner.Set(child, value);
                    var key = owner.Add(new Disposable(() => viewControl.Unset()));

                    switch (strategy)
                    {
                        case OptionalStrategy.Activity:
                            child.gameObject.SetActive(true);
                            break;
                        case OptionalStrategy.Nothing:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    return (viewControl, key).Some();
                }, () =>
                {
                    switch (strategy)
                    {
                        case OptionalStrategy.Activity:
                            child.gameObject.SetActive(false);
                            break;
                        case OptionalStrategy.Nothing:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    return Option.None<(ViewControl<TView, TModel>, ISubscriptions.Key)>();
                });
            }

            var viewControl = ViewControl(observableModel.Value);
            var unsubscription = observableModel.Subscribe(option =>
            {
                viewControl.MatchSome(tuple =>
                {
                    owner.Remove(tuple.key);
                    tuple.viewControl.Unset();
                });

                viewControl = ViewControl(option);
            });

            var key = owner.Add(unsubscription);
            return new Disposable(() =>
            {
                owner.Remove(key);
                unsubscription.Dispose();
            });
        }

        internal static (TChildView view, IDisposable unsubscription) Create<TChildView, TChildModel>(
            this ISubscriptions owner, TChildView childPrefab, IReadOnlyObservable<TChildModel> observableModel,
            RectTransform parent)
            where TChildView : IView<TChildModel>
        {
            var view = (TChildView)childPrefab.Create(parent);
            return (view, owner.Set(view, observableModel));
        }

        internal static (TChildView view, IDisposable unsubscription) Create<TView,
            TChildView, TChildModel>(
            this ISubscriptions owner, TChildView childPrefab, IReadOnlyObservable<TChildModel> observableModel,
            TView parent)
            where TView : MonoBehaviour, IView
            where TChildView : IView<TChildModel>
        {
            return Create(owner, childPrefab, observableModel, (RectTransform)parent.transform);
        }

        public enum OptionalStrategy
        {
            Activity,
            Nothing
        }

        private class ReplacableDisposable : IDisposable
        {
            private IDisposable _disposable;

            public void Replace(IDisposable disposable)
            {
                _disposable = disposable;
            }

            public void Dispose()
            {
                _disposable?.Dispose();
            }
        }
    }
}