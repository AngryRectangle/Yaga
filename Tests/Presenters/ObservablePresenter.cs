using System;
using Yaga;

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

        protected override void OnModelSet(TView view, TModel model)
        {
            _onModelSet?.Invoke(view);
        }

        protected override void OnModelUnset(TView view)
        {
            _onModelUnset?.Invoke(view);
        }
    }
}