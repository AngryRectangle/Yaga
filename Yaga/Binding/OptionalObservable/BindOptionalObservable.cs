using System;

namespace Yaga.Binding.OptionalObservable
{
    public interface IBindOptionalObservable<T>
    {
        IBindAccessor To(IView<T> view);
        IBindAccessor To(Action<T> onSet, Action onDefault);
        IBindOptionalObservable<T2> As<T2>(Func<T, T2> converter);
        T Data { get; }
    }

    public class BindOptionalObservable<T> : IBindOptionalObservable<T>
    {
        private readonly BindingContext _context;
        private readonly Utils.IOptionalObservable<T> _observable;
        private readonly Func<T> _dataAccessor;
        private readonly Func<bool> _defaultAccessor;
        private Action _onDispose;
        private Action _onDefaultAction;
        public T Data => _dataAccessor();

        public BindOptionalObservable(
            BindingContext context,
            Utils.IOptionalObservable<T> observable,
            Action onDispose = default
        )
        {
            _context = context;
            _observable = observable;
            _onDispose = onDispose;
            _dataAccessor = () => observable.Data;
            _defaultAccessor = () => observable.IsDefault;
        }

        public IBindAccessor To(IView<T> view)
        {
            void Action()
            {
                if (view.IsOpened) view.Close();
                UiBootstrap.Instance.Unset(view);
            }

            _onDefaultAction += Action;
            
            var accessor = new BindAccessor(() =>
            {
                if (!view.IsInstanced)
                    view.Create();

                if (!_defaultAccessor())
                {
                    if (!view.IsOpened)
                        view.Open();
                    
                    UiBootstrap.Instance.Set(view, _dataAccessor());
                }
                else
                {
                    _onDefaultAction();
                }
            }, _onDispose);
            
            _context._bindings.Add(accessor);
            return accessor;
        }

        public IBindAccessor To(Action<T> onSet, Action onDefault)
        {
            _onDefaultAction += onDefault;
            return new BindAccessor(() =>
            {
                if (_defaultAccessor())
                    onDefault();
                else
                    onSet(_dataAccessor());

            }, _onDispose);
        }

        public IBindOptionalObservable<T1> As<T1>(Func<T, T1> converter)
        {
            var converted = new ConvertedOptionalObservable<T1>(() => converter(_dataAccessor()), _defaultAccessor);
            var reflector = _observable.Subscribe(e => converted.Perform(converter(e)), _onDefaultAction);
            _onDispose += reflector.Dispose;
            return new BindOptionalObservable<T1>(_context, converted);
        }
    }
}