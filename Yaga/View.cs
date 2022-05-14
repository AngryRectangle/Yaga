using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Yaga.Binding;
using Yaga.Utils;

namespace Yaga
{
    public abstract class View<TModel> : BaseView<TModel>
    {
        [SerializeField] private Bind[] _bindings;

        private BindingContext _context = new BindingContext();
        public BindingContext Context => _context;
        public IReadOnlyCollection<Bind> Bindings => _bindings;

        public override IEnumerable<IView> Children => Array.Empty<IView>();

        private List<IDisposable> _disposables = new List<IDisposable>();

        /// <summary>
        /// Subscribe on observable value change and dispose subscription when needed.
        /// </summary>
        public void Subscribe<T>(Utils.IObservable<T> observable, Action<T> action)
            => _disposables.Add(observable.Subscribe(action));

        /// <summary>
        /// Subscribe on observable value change and dispose subscription when needed.
        /// </summary>
        public void Subscribe<T>(IOptionalObservable<T> observable, Action<T> action, Action onNull)
            => _disposables.Add(observable.Subscribe(action, onNull));

        /// <summary>
        /// Subscribe on beacon and dispose subscription when needed.
        /// </summary>
        public void Subscribe(Beacon observable, Action action)
            => _disposables.Add(observable.Add(action));

        public void Subscribe(UnityEvent @event, UnityAction action)
        {
            @event.AddListener(action);
            _disposables.Add(new Reflector(() => @event.RemoveListener(action)));
        }

        /// <summary>
        /// Subscribe on beacon and dispose subscription when needed.
        /// </summary>
        public void Subscribe<T1, T2>(Beacon<T1, T2> observable, Action<T1, T2> action)
            => _disposables.Add(observable.Add(action));

        /// <summary>
        /// Subscribe on beacon and dispose subscription when needed.
        /// </summary>
        public void Subscribe<T1, T2, T3>(Beacon<T1, T2, T3> observable, Action<T1, T2, T3> action)
            => _disposables.Add(observable.Add(action));

        /// <summary>
        /// Subscribe on beacon and dispose subscription when needed.
        /// </summary>
        public void Subscribe<T>(Beacon<T> observable, Action<T> action)
            => _disposables.Add(observable.Add(action));

        /// <summary>
        /// Subscribe on observable value change, execute action and dispose subscription when needed.
        /// </summary>
        public void SubscribeAndCall<T>(Utils.IObservable<T> observable, Action<T> action)
        {
            _disposables.Add(observable.Subscribe(action));
            action(observable.Data);
        }

        /// <summary>
        /// Subscribe on observable value change and dispose subscription when needed.
        /// </summary>
        public void SubscribeAndCall<T>(IOptionalObservable<T> observable, Action<T> action, Action onNull)
        {
            _disposables.Add(observable.Subscribe(action, onNull));
            if (observable.IsDefault)
                onNull();
            else
                action(observable.Data);
        }

        public void AddDisposable(IDisposable disposable) => _disposables.Add(disposable);

        public override void Close()
        {
            base.Close();
            foreach (var disposable in _disposables) disposable.Dispose();
            _disposables.Clear();

            foreach (var bind in _bindings) bind.View.Close();
            Context.Dispose();
        }

        public override void Create()
        {
            base.Create();
            foreach (var bind in _bindings) bind.View.Create();
        }

        public override void Open()
        {
            base.Open();
            foreach (var bind in _bindings)
            {
                if (bind.Type != Bind.BindType.OptionalObservableField)
                    bind.View.Open();
            }
        }
    }
}