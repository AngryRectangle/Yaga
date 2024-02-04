using Optional;

namespace Yaga
{
    public class ViewControl<TView, TModel> where TView : IView<TModel>
    {
        private readonly TView _view;
        private Option<Subscriptions> _owner;

        public TView View => _view;
        public Option<ISubscriptionsOwner> Subs => _owner.Map(e => (ISubscriptionsOwner)e);

        public ViewControl(TView view, Subscriptions owner)
        {
            _view = view;
            _owner = owner.Some();
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