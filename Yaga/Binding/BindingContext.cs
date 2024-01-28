using System;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.UI;
using Yaga.Binding.Observable;
using Yaga.Binding.Observable.ConcreteObservables;
using Yaga.Utils;

namespace Yaga.Binding
{
    // context.Bind(model.Building).To(view.text).As(e=>e.ToString()).Perform();
    // context.Bind(model.Building).To(view.text).Perform();
    // context.Bind(model.button.onClick).To(view.text).As(()=>DoThings()).Perform();
    // context.Bind(model.button.onClick).As(()=>DoThings()).Perform();

    public class BindingContext : IDisposable
    {
        internal List<BindAccessor> _bindings = new List<BindAccessor>();

        public BindObservable<T> Bind<T>(Utils.IObservable<T> observable)
            => new BindObservable<T>(this, observable);

        public BindBeacon Bind(Beacon beacon)
            => new BindBeacon(this, beacon);

        public BindBeacon Bind(UnityEvent @event)
        {
            var beacon = new Beacon();
            var action = new UnityAction(() => beacon.Execute());
            @event.AddListener(action);
            return new BindBeacon(this, beacon, () => @event.RemoveListener(action));
        }
        
        public BindBeacon Bind(Button button) => Bind(button.onClick);

        public BindStringObservable Bind(Utils.IObservable<string> observable)
            => new BindStringObservable(this, observable);
        public BindIntObservable Bind(Utils.IObservable<int> observable)
            => new BindIntObservable(this, observable);

        public void Dispose()
        {
            foreach (var binding in _bindings) binding.Dispose();
            _bindings.Clear();
        }
    }

    public class BindAccessor : IBindAccessor
    {
        private readonly Action _perform;
        private readonly Action _disposable;

        public BindAccessor(Action perform, Action disposable)
        {
            _perform = perform;
            _disposable = disposable;
        }

        public void Dispose() => _disposable();

        public void Execute() => _perform();
    }

    public interface IBindAccessor : IDisposable
    {
        void Execute();
    }
}