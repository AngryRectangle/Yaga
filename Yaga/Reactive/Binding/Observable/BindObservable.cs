using System;
using Yaga.Reactive.Binding.Observable.ConcreteObservables;

namespace Yaga.Reactive.Binding.Observable
{
    public class BindObservable<T>
    {
        protected readonly BindingContext Context;
        private readonly IReadOnlyObservable<T> _observable;
        private readonly Func<T> _dataAccessor;
        protected Action OnDispose;

        public T Data => _dataAccessor();

        public BindObservable(
            BindingContext context,
            IReadOnlyObservable<T> observable,
            Action onDispose = default)
        {
            Context = context;
            _observable = observable;
            OnDispose = onDispose;
            _dataAccessor = () => observable.Data;
        }

        public IBindAccessor To(Action<T> action)
        {
            var accessor = new BindAccessor(() => action(_dataAccessor()), OnDispose);
            Context._bindings.Add(accessor);
            return accessor;
        }

        private ConvertedObservable<T1> GetConverted<T1>(Func<T, T1> converter)
        {
            var converted = new ConvertedObservable<T1>(() => converter(_dataAccessor()));
            var reflector = _observable.Subscribe(e => converted.Perform(converter(e)));
            OnDispose += reflector.Dispose;
            return converted;
        }

        public BindObservable<T1> As<T1>(Func<T, T1> converter)
            => new BindObservable<T1>(Context, GetConverted(converter), OnDispose);

        public BindStringObservable As(Func<T, string> converter) =>
            new BindStringObservable(Context, GetConverted(converter), OnDispose);

        public BindStringObservable AsString() =>
            new BindStringObservable(Context, GetConverted(value => value.ToString()), OnDispose);
    }
}