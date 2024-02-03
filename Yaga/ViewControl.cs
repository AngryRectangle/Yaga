using System;
using Optional;
using Yaga.Exceptions;

namespace Yaga
{
    public class ViewControl<TView, TModel> : ISubscriptionsOwner
        where TView : IView<TModel>
    {
        private readonly TView _view;
        private Option<Subscriptions> _owner;

        public TView View => _view;

        public ViewControl(TView view, Subscriptions owner)
        {
            _view = view;
            _owner = owner.Some();
        }

        public ISubscriptionsOwner.Key Add(IDisposable disposable)
        {
            return _owner.ValueOr(() => throw new ViewModelIsUnsetException()).Add(disposable);
        }

        public bool Remove(ISubscriptionsOwner.Key key)
        {
            return _owner.ValueOr(() => throw new ViewModelIsUnsetException()).Remove(key);
        }

        public void Set(TModel model)
        {
            _owner = UiBootstrap.Instance.Set(_view, model).Some();
        }

        public void Unset()
        {
            _owner.MatchSome(subs => subs.Dispose());
            _owner = Option.None<Subscriptions>();
        }
    }
}