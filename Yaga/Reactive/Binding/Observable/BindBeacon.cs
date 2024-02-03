using System;
using Yaga.Reactive.Binding.Observable.ConcreteObservables;

namespace Yaga.Reactive.Binding.Observable
{
    public class BindBeacon
    {
        protected readonly BindingContext Context;
        protected Action OnDispose;
        private readonly Beacon _beacon;

        public BindBeacon(BindingContext context, Beacon beacon, Action onDispose = default)
        {
            Context = context;
            _beacon = beacon;
            OnDispose = onDispose;
        }
        
        public IBindAccessor To(Action action)
        {
            var accessor = new BindAccessor(action, OnDispose);
            Context._bindings.Add(accessor);
            return accessor;
        }

        private ConvertedObservable<T1> GetConverted<T1>(Func<T1> converter)
        {
            var converted = new ConvertedObservable<T1>(converter);
            var reflector = _beacon.Add(() => converted.Perform(converter()));
            OnDispose += reflector.Dispose;
            return converted;
        }

        public BindObservable<T1> As<T1>(Func<T1> converter) 
            => new BindObservable<T1>(Context, GetConverted(converter), OnDispose);
        
        public BindStringObservable As(Func<string> converter) => 
            new BindStringObservable(Context, GetConverted(converter), OnDispose);
    }
}