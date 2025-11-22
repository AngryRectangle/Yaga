using System;
using UnityEngine.Events;

namespace Yaga.Reactive.BeaconExtensions
{
    public static class BeaconUiExtensions
    {
        /// <summary>
        /// Connects a parameterless Unity <see cref="UnityEvent"/> to a <see cref="Beacon"/> so that
        /// every time the event is invoked, the beacon executes.
        /// </summary>
        public static IDisposable Is(this Beacon signal, UnityEvent @event)
        {
            @event.AddListener(signal.Execute);
            return new Disposable(() => @event.RemoveListener(signal.Execute));
        }
        
        /// <summary>
        /// Connects a typed Unity <see cref="UnityEvent{T0}"/> to a typed <see cref="Beacon{T}"/> so that
        /// every time the event is invoked, the beacon executes with the event argument.
        /// </summary>
        public static IDisposable Is<T>(this Beacon<T> signal, UnityEvent<T> @event)
        {
            @event.AddListener(signal.Execute);
            return new Disposable(() => @event.RemoveListener(signal.Execute));
        }
        
        /// <summary>
        /// Connects a parameterless Unity <see cref="UnityEvent"/> to a <see cref="Beacon"/> so that
        /// every time the event is invoked, the beacon executes.
        /// </summary>
        public static IDisposable Is<T>(this Beacon<T> signal, UnityEvent @event, Func<T> valueProvider)
        {
            void Handler() => signal.Execute(valueProvider());
            @event.AddListener(Handler);
            return new Disposable(() => @event.RemoveListener(Handler));
        }
    }
}