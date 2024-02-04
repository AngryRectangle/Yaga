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

            var key = owner.Add(unsubscription);
            return new Disposable(() =>
            {
                owner.Remove(key);
                unsubscription.Dispose();
            });
        }

        public static IDisposable Set<TView, TModel>(this ISubscriptions owner, TView child,
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

        public enum OptionalStrategy
        {
            Activity,
            Nothing
        }
    }
}