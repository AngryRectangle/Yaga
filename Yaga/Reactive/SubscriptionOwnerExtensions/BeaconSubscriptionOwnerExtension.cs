using System;

namespace Yaga.Reactive
{
    public static class BeaconSubscriptionOwnerExtension
    {
        /// <summary>
        /// Subscribe on beacon and dispose subscription when needed.
        /// </summary>
        public static void Subscribe(this ISubscriptions owner, Beacon observable, Action action)
            => owner.Add(observable.Add(action));

        /// <inheritdoc cref="Subscribe(ISubscriptions,Beacon,Action)"/>
        public static void Subscribe<T>(this ISubscriptions owner, Beacon<T> observable, Action<T> action)
            => owner.Add(observable.Add(action));

        /// <inheritdoc cref="Subscribe(ISubscriptions,Beacon,Action)"/>
        public static void Subscribe<T1, T2>(this ISubscriptions owner, Beacon<T1, T2> observable,
            Action<T1, T2> action)
            => owner.Add(observable.Add(action));

        /// <inheritdoc cref="Subscribe(ISubscriptions,Beacon,Action)"/>
        public static void Subscribe<T1, T2, T3>(this ISubscriptions owner, Beacon<T1, T2, T3> observable,
            Action<T1, T2, T3> action)
            => owner.Add(observable.Add(action));
    }
}