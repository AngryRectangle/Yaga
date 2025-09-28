using System;

namespace Yaga
{
    public static class DisposableExtension
    {
        /// <summary>
        /// Registers the given <paramref name="disposable"/> with the specified <paramref name="owner"/> so that
        /// it will be disposed when the owner is disposed. This is convenience sugar for
        /// <see cref="ISubscriptions.Add(System.IDisposable)"/> and enables the fluent
        /// "<c>… .AddTo(subs)</c>" pattern in bindings.
        /// </summary>
        /// <param name="disposable">The disposable resource to be owned. Must not be <c>null</c>.</param>
        /// <param name="owner">
        /// The subscriptions container that assumes ownership of <paramref name="disposable"/> and will dispose it
        /// as part of its own teardown. Must not be <c>null</c>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="disposable"/> is <c>null</c> or if <paramref name="owner"/> is <c>null</c>.
        /// </exception>
        /// <remarks>
        /// Typical usage ties the lifetime of a subscription to a presenter/view scope:
        /// <code>
        /// model.Text.Into(view.Label).AddTo(subs);
        /// model.IsSelected.IntoActive(view.Highlight).AddTo(subs);
        /// </code>
        /// If <paramref name="owner"/> has already been disposed, the behavior is determined by
        /// <see cref="ISubscriptions.Add(System.IDisposable)"/> (e.g., it may dispose the disposable immediately).
        /// Calling <c>AddTo</c> more than once with the same <paramref name="disposable"/> may register it multiple times,
        /// depending on the <see cref="ISubscriptions"/> implementation.
        /// </remarks>
        public static void AddTo(this IDisposable disposable, ISubscriptions owner)
        {
            if (disposable == null)
                throw new ArgumentNullException(nameof(disposable));
            if (owner == null)
                throw new ArgumentNullException(nameof(owner));

            owner.Add(disposable);
        }
    }
}