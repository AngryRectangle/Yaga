using System;
using System.Collections.Generic;
using Yaga.Binding.Observable;
using Yaga.Binding.OptionalObservable;

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
        
        public BindStringObservable Bind(Utils.IObservable<string> observable)
            => new BindStringObservable(this, observable);
        
        public IBindOptionalObservable<T> Bind<T>(Utils.IOptionalObservable<T> observable)
            => new BindOptionalObservable<T>(this, observable);

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