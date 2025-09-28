using System;
using Optional;
using TMPro;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Yaga.Reactive.ObservableExtensions
{
    public static class ObservableUiExtension
    {
        private static bool IsAlive(Object o) => o != null;
        private static bool IsAlive(TMP_Text t) => t != null && t.gameObject != null;

        /// <summary>
        /// Binds a boolean observable to the <see cref="Component"/>'s <see cref="GameObject.activeSelf"/> state.
        /// </summary>
        /// <param name="observable">Source of active/inactive values.</param>
        /// <param name="monoBehaviour">Any component whose <see cref="Component.gameObject"/> will be toggled.</param>
        /// <returns>
        /// An <see cref="IDisposable"/> that, when disposed, stops listening for further changes.
        /// Disposing multiple times is a no-op.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="monoBehaviour"/> is <c>null</c>.
        /// </exception>
        /// <remarks>
        /// Initializes the GameObject’s active state from the observable’s current value,
        /// then updates on subsequent emissions. If the GameObject is destroyed later,
        /// the subscription is disposed automatically and further updates are ignored.
        /// Assumes calls are made on Unity’s main thread.
        /// </remarks>
        public static IDisposable IntoActive(this IReadOnlyObservable<bool> observable, Component monoBehaviour)
        {
            if (monoBehaviour == null)
                throw new ArgumentNullException(nameof(monoBehaviour));

            var gameObject = monoBehaviour.gameObject;
            return observable.IntoActive(gameObject);
        }

        /// <summary>
        /// Binds a boolean observable to a <see cref="GameObject"/>'s active state.
        /// </summary>
        /// <param name="observable">Source of active/inactive values.</param>
        /// <param name="gameObject">Target GameObject whose active state is controlled by the observable.</param>
        /// <returns>
        /// An <see cref="IDisposable"/> that, when disposed, stops listening for further changes.
        /// Disposing multiple times is a no-op.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="gameObject"/> is <c>null</c>.
        /// </exception>
        /// <remarks>
        /// Sets the initial active state to the observable’s current value, then reacts to updates.
        /// If the <paramref name="gameObject"/> is destroyed later, the subscription is disposed automatically.
        /// Assumes calls are made on Unity’s main thread.
        /// </remarks>
        public static IDisposable IntoActive(this IReadOnlyObservable<bool> observable, GameObject gameObject)
        {
            if (gameObject == null)
                throw new ArgumentNullException(nameof(gameObject));

            var guard = new SubGuard();

            gameObject.SetActive(observable.Value);
            guard.Attach(observable.Subscribe(isActive =>
            {
                if (guard.StopIfDead(gameObject)) return;
                gameObject.SetActive(isActive);
            }));

            return guard;
        }

        /// <summary>
        /// Binds a string observable to a <see cref="TMP_Text"/>'s <see cref="TMP_Text.text"/>.
        /// </summary>
        /// <param name="observable">Source of string values.</param>
        /// <param name="text">Target <see cref="TMP_Text"/>.</param>
        /// <returns>
        /// An <see cref="IDisposable"/> that stops the updates when disposed.
        /// Disposing multiple times is a no-op.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="text"/> is <c>null</c> or Unity-destroyed at bind time.
        /// </exception>
        /// <remarks>
        /// Initializes the text from the observable’s current value, then updates on subsequent emissions.
        /// If <paramref name="text"/> is destroyed later, the subscription is disposed automatically and further updates are ignored.
        /// Assumes calls are made on Unity’s main thread.
        /// </remarks>
        public static IDisposable Into(this IReadOnlyObservable<string> observable, TMP_Text text)
        {
            if (!IsAlive(text))
                throw new ArgumentNullException(nameof(text));

            var guard = new SubGuard();

            text.text = observable.Value;
            guard.Attach(observable.Subscribe(value =>
            {
                if (guard.StopIfDead(text)) return;
                text.text = value;
            }));

            return guard;
        }

        /// <summary>
        /// Binds a non-string observable to a <see cref="TMP_Text"/> by rendering values with <see cref="object.ToString"/>.
        /// </summary>
        /// <typeparam name="T">Value type.</typeparam>
        /// <param name="observable">Source of values.</param>
        /// <param name="text">Target <see cref="TMP_Text"/>.</param>
        /// <returns>
        /// An <see cref="IDisposable"/> that stops the updates when disposed.
        /// Disposing multiple times is a no-op.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="text"/> is <c>null</c> or Unity-destroyed at bind time.
        /// </exception>
        /// <remarks>
        /// Initializes the text from the observable’s current value using <see cref="object.ToString"/>,
        /// then updates on subsequent emissions. If <paramref name="text"/> is destroyed later,
        /// the subscription is disposed automatically.
        /// Assumes calls are made on Unity’s main thread.
        /// </remarks>
        public static IDisposable Into<T>(this IReadOnlyObservable<T> observable, TMP_Text text)
        {
            if (!IsAlive(text))
                throw new ArgumentNullException(nameof(text));

            var guard = new SubGuard();

            text.text = observable.Value?.ToString();

            guard.Attach(observable.Subscribe(value =>
            {
                if (guard.StopIfDead(text)) return;
                text.text = value?.ToString();
            }));

            return guard;
        }

        /// <summary>
        /// Binds an <see cref="Optional.Option{T}"/> to a <see cref="TMP_Text"/>:
        /// when <c>Some</c>, shows the text and sets its content; when <c>None</c>, hides the text’s GameObject.
        /// </summary>
        /// <typeparam name="T">Underlying value type.</typeparam>
        /// <param name="observable">Source of optional values.</param>
        /// <param name="text">Target <see cref="TMP_Text"/> and its GameObject for visibility control.</param>
        /// <returns>
        /// An <see cref="IDisposable"/> that stops the updates when disposed.
        /// Disposing multiple times is a no-op.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="text"/> is <c>null</c> or Unity-destroyed at bind time.
        /// </exception>
        /// <remarks>
        /// Initializes visibility/content from the current value, then reacts to updates.
        /// If <paramref name="text"/> is destroyed later, the subscription is disposed automatically.
        /// Assumes calls are made on Unity’s main thread.
        /// </remarks>
        public static IDisposable Into<T>(this IReadOnlyObservable<Option<T>> observable, TMP_Text text)
        {
            if (!IsAlive(text))
                throw new ArgumentNullException(nameof(text));

            var guard = new SubGuard();

            observable.Value.Match(
                some =>
                {
                    if (guard.StopIfDead(text)) return;
                    text.gameObject.SetActive(true);
                    text.text = some?.ToString();
                },
                () =>
                {
                    if (guard.StopIfDead(text)) return;
                    text.gameObject.SetActive(false);
                });

            guard.Attach(observable.Subscribe(opt =>
            {
                if (guard.StopIfDead(text)) return;

                opt.Match(
                    some =>
                    {
                        if (guard.StopIfDead(text)) return;
                        text.gameObject.SetActive(true);
                        text.text = some?.ToString();
                    },
                    () =>
                    {
                        if (guard.StopIfDead(text)) return;
                        text.gameObject.SetActive(false);
                    });
            }));

            return guard;
        }

        /// <summary>
        /// Binds a model observable to a Yaga <see cref="View{TModel}"/> via <see cref="UiBootstrap"/>.
        /// Each emission replaces the previous per-model binding (“latest binding wins”).
        /// </summary>
        /// <typeparam name="TModel">Model type.</typeparam>
        /// <typeparam name="TView">Concrete view type deriving from <see cref="View{TModel}"/>.</typeparam>
        /// <param name="observable">Source of models.</param>
        /// <param name="view">Target view instance.</param>
        /// <returns>
        /// An <see cref="IDisposable"/> representing the whole binding lifetime.
        /// Disposing cancels the upstream subscription and disposes the current per-model binding; it is idempotent.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="view"/> is <c>null</c> or Unity-destroyed at bind time.
        /// </exception>
        /// <remarks>
        /// Initializes the view with the current model. The <see cref="ISubscriptions"/> returned from
        /// <see cref="UiBootstrap.Instance"/>.<see cref="UiBootstrap.Set{TView,TModel}(TView,TModel)"/>
        /// is hooked so that when the view is destroyed and its subscriptions are disposed,
        /// the entire chain is torn down and further updates are ignored.
        /// Assumes calls are made on Unity’s main thread.
        /// </remarks>
        public static IDisposable Into<TModel, TView>(this IReadOnlyObservable<TModel> observable, TView view)
            where TView : View<TModel>
        {
            var currentBinding = new ReplacableDisposable();
            IDisposable subscription = null;
            var disposed = false;

            bool IsViewAlive() => IsAlive(view) && view.gameObject != null;
            if (!IsViewAlive())
                throw new ArgumentNullException(nameof(view));

            void DisposeAll()
            {
                if (disposed) return;
                disposed = true;
                try
                {
                    subscription?.Dispose();
                }
                catch
                {
                    /* ignore */
                }

                try
                {
                    currentBinding.Dispose();
                }
                catch
                {
                    /* ignore */
                }
            }

            var initial = UiBootstrap.Instance.Set(view, observable.Value);
            currentBinding.Replace(initial);
            initial.Add(new Disposable(DisposeAll));

            subscription = observable.Subscribe(model =>
            {
                var subs = UiBootstrap.Instance.Set(view, model);
                currentBinding.Replace(subs);
                subs.Add(new Disposable(DisposeAll));
            });

            return new Disposable(DisposeAll);
        }

        /// <summary>
        /// Binds an optional model to a Yaga <see cref="View{TModel}"/>.
        /// When <c>Some</c>, creates/updates the binding and activates the view GameObject;
        /// when <c>None</c>, clears the binding and deactivates the view GameObject.
        /// Safe against emissions after the view is destroyed.
        /// </summary>
        /// <typeparam name="TModel">Model type.</typeparam>
        /// <typeparam name="TView">Concrete view type deriving from <see cref="View{TModel}"/>.</typeparam>
        /// <param name="observable">Source of optional models.</param>
        /// <param name="view">Target view instance.</param>
        /// <returns>
        /// An <see cref="IDisposable"/> representing the whole binding lifetime.
        /// Disposing cancels the upstream subscription and disposes the current binding; it is idempotent.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="view"/> is <c>null</c> or Unity-destroyed at bind time.
        /// </exception>
        /// <remarks>
        /// If the view is destroyed at any point, the binding disposes itself and ignores further updates.
        /// Per-model bindings hook the view’s <see cref="ISubscriptions"/> so destruction tears down the chain.
        /// Assumes calls are made on Unity’s main thread.
        /// </remarks>
        public static IDisposable Into<TModel, TView>(
            this IReadOnlyObservable<Option<TModel>> observable,
            TView view)
            where TView : View<TModel>
        {
            var currentBinding = new ReplacableDisposable();
            IDisposable subscription = null;
            var disposed = false;

            bool IsViewAlive() => IsAlive(view) && view.gameObject != null;
            if (!IsViewAlive())
                throw new ArgumentNullException(nameof(view));

            void DisposeAll()
            {
                if (disposed) return;
                disposed = true;
                try
                {
                    subscription?.Dispose();
                }
                catch
                {
                    /* ignore */
                }

                try
                {
                    currentBinding.Dispose();
                }
                catch
                {
                    /* ignore */
                }
            }

            // empty disposable used to clear previous binding on None
            static IDisposable Empty() => new Disposable(() => { });

            // 1) Initialize from current value, but only if the view is still alive
            if (IsViewAlive())
            {
                observable.Value.Match(
                    value =>
                    {
                        var subs = UiBootstrap.Instance.Set(view, value);
                        currentBinding.Replace(subs);
                        // If the view is destroyed later, tear down everything (may run multiple times, safe)
                        subs.Add(new Disposable(DisposeAll));
                        view.gameObject.SetActive(true);
                    },
                    () =>
                    {
                        currentBinding.Replace(Empty());
                        view.gameObject.SetActive(false);
                    }
                );
            }
            else
            {
                // View already destroyed — nothing to do; ensure we’re inert.
                currentBinding.Replace(Empty());
            }

            // 2) Subscribe to future updates
            subscription = observable.Subscribe(opt =>
            {
                // If the view has been destroyed at any time, stop and never touch it again
                if (!IsViewAlive())
                {
                    DisposeAll();
                    return;
                }

                opt.Match(
                    value =>
                    {
                        var subs = UiBootstrap.Instance.Set(view, value);
                        currentBinding.Replace(subs);
                        subs.Add(new Disposable(DisposeAll));
                        view.gameObject.SetActive(true);
                    },
                    () =>
                    {
                        view.gameObject.SetActive(false);
                        currentBinding.Replace(Empty());
                    }
                );
            });

            // 3) Returned disposable cancels updates and disposes the latest binding; second call is a no-op.
            return new Disposable(DisposeAll);
        }
        
        /// <summary>
        /// Mirrors values from one observable into another.
        /// </summary>
        /// <typeparam name="TModel">Value type.</typeparam>
        /// <param name="observable">Source observable to read from.</param>
        /// <param name="toObservable">Target observable that receives mirrored values.</param>
        /// <returns>
        /// An <see cref="IDisposable"/> that, when disposed, stops mirroring subsequent updates.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="toObservable"/> is <c>null</c>.
        /// </exception>
        /// <remarks>
        /// Immediately assigns the current value from <paramref name="observable"/> to <paramref name="toObservable"/>,
        /// then forwards all subsequent updates. The returned disposable is provided by the source observable;
        /// double-dispose is typically a no-op in standard implementations, but is not guaranteed by this method.
        /// </remarks>
        public static IDisposable Into<TModel>(this IReadOnlyObservable<TModel> observable,
            Observable<TModel> toObservable)
        {
            if (toObservable == null)
                throw new ArgumentNullException(nameof(toObservable));

            toObservable.Value = observable.Value;
            return observable.Subscribe(model => toObservable.Value = model);
        }

        private sealed class SubGuard : IDisposable
        {
            private IDisposable _sub;
            private bool _disposed;

            public void Attach(IDisposable sub)
            {
                if (_disposed)
                {
                    sub?.Dispose();
                    return;
                }

                _sub = sub;
            }

            public void Dispose()
            {
                if (_disposed) return;
                _disposed = true;
                try
                {
                    _sub?.Dispose();
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }

                _sub = null;
            }

            /// <summary>
            /// If target is destroyed, dispose and indicate the caller should bail out.
            /// </summary>
            public bool StopIfDead(Object o)
            {
                if (_disposed) return true;
                if (!IsAlive(o))
                {
                    Dispose();
                    return true;
                }

                return false;
            }

            public bool IsDisposed => _disposed;
        }
    }
}