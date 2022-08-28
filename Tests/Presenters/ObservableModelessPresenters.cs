using System;
using Yaga;

namespace Tests.Presenters
{
    internal class ObservableModelessPresenter<TView> : Presenter<TView>
        where TView : View
    {
        private readonly Action<TView> _onModelSet;
        private readonly Action<TView> _onModelUnset;

        public ObservableModelessPresenter(Action<TView> onModelSet = null, Action<TView> onModelUnset = null)
        {
            _onModelSet = onModelSet;
            _onModelUnset = onModelUnset;
        }

        protected override void OnSet(TView view)
        {
            _onModelSet?.Invoke(view);
        }

        protected override void OnUnset(TView view)
        {
            _onModelUnset?.Invoke(view);
        }
    }
}