using System;
using Yaga;
using Yaga.Reactive;

namespace Tests.Presenters
{
    internal class ObservablePresenter<TView, TModel> : Presenter<TView, TModel>
        where TView : IView<TModel>
    {
        private readonly Action<TView> _onModelSet;
        private readonly Action<TView> _onModelUnset;

        public ObservablePresenter(Action<TView> onModelSet = null, Action<TView> onModelUnset = null)
        {
            _onModelSet = onModelSet;
            _onModelUnset = onModelUnset;
        }

        protected override void OnSet(TView view, TModel model, ISubscriptionsOwner subs)
        {
            _onModelSet?.Invoke(view);
        }

        protected override void OnUnset(TView view)
        {
            _onModelUnset?.Invoke(view);
        }
    }
    
    internal class ObservablePresenter<TView> : Presenter<TView>
        where TView : IView<Unit>
    {
        private readonly Action<TView> _onModelSet;
        private readonly Action<TView> _onModelUnset;

        public ObservablePresenter(Action<TView> onModelSet = null, Action<TView> onModelUnset = null)
        {
            _onModelSet = onModelSet;
            _onModelUnset = onModelUnset;
        }

        protected override void OnSet(TView view, ISubscriptionsOwner subs)
        {
            _onModelSet?.Invoke(view);
        }

        protected override void OnUnset(TView view)
        {
            _onModelUnset?.Invoke(view);
        }
    }
}