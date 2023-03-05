using System;
using Yaga.Utils;

namespace Yaga.Extensions
{
    public static class ViewEventSubscriptionExtension
    {
        /// <summary>
        /// Subscribe to the action event and dispose subscription when needed.
        /// </summary>
        /// <param name="unsubscription">Invoked when action needs to be unsubscribed</param>
        public static void SubscribeEvent(this View view, ref Action @event, Action<Action> unsubscription,
            Action action)
        {
            @event += action;
            view.AddUnsubscription(() => unsubscription(action));
        }

        /// <summary>
        /// Subscribe to the action event with one parameter and dispose subscription when needed.
        /// </summary>
        /// <param name="unsubscription">Invoked when action needs to be unsubscribed</param>
        public static void SubscribeEvent<T1>(this View view, ref Action<T1> @event, Action<Action<T1>> unsubscription,
            Action<T1> action)
        {
            @event += action;
            view.AddUnsubscription(() => unsubscription(action));
        }

        /// <summary>
        /// Subscribe to the action event with two parameters and dispose subscription when needed.
        /// </summary>
        /// <param name="unsubscription">Invoked when action needs to be unsubscribed</param>
        public static void SubscribeEvent<T1, T2>(this View view, ref Action<T1, T2> @event,
            Action<Action<T1, T2>> unsubscription, Action<T1, T2> action)
        {
            @event += action;
            view.AddUnsubscription(() => unsubscription(action));
        }

        /// <summary>
        /// Subscribe to the action event with three parameters and dispose subscription when needed.
        /// </summary>
        /// <param name="unsubscription">Invoked when action needs to be unsubscribed</param>
        public static void SubscribeEvent<T1, T2, T3>(this View view, ref Action<T1, T2, T3> @event,
            Action<Action<T1, T2, T3>> unsubscription, Action<T1, T2, T3> action)
        {
            @event += action;
            view.AddUnsubscription(() => unsubscription(action));
        }

        /// <summary>
        /// Subscribe to the action event with four parameters and dispose subscription when needed.
        /// </summary>
        /// <param name="unsubscription">Invoked when action needs to be unsubscribed</param>
        public static void SubscribeEvent<T1, T2, T3, T4>(this View view, ref Action<T1, T2, T3, T4> @event,
            Action<Action<T1, T2, T3, T4>> unsubscription, Action<T1, T2, T3, T4> action)
        {
            @event += action;
            view.AddUnsubscription(() => unsubscription(action));
        }

        /// <summary>
        /// Subscribe on custom event type and dispose subscription when needed.
        /// </summary>
        /// <param name="view"></param>
        /// <param name="subscription">Invoked when action needs to be subscribed</param>
        /// <param name="unsubscription">Invoked when action needs to be unsubscribed</param>
        /// <param name="action">Action to be subscribed</param>
        /// <typeparam name="TEvent">Custom event type</typeparam>
        public static void SubscribeEvent<TEvent>(this View view, Action<TEvent> subscription,
            Action<TEvent> unsubscription, TEvent action)
        {
            subscription(action);
            view.AddUnsubscription(() => unsubscription(action));
        }
    }
}