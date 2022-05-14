using System;

namespace Yaga
{
    public interface IPresenter<in TView, in TModel> : IPresenter
        where TView : IView<TModel>
    {
        void Set(TView view, TModel model);
        void Unset(TView view);
    }

    public interface IPresenter
    {
        void Unset(IView view);
        bool AcceptableView(Type viewType);
    }

    public abstract class Presenter<TView, TModel> : IPresenter<TView, TModel>
        where TView : IView<TModel>
    {
        public void Set(TView view, TModel model)
        {
            if (view.HasModel)
            {
                if (model.Equals(view.Model))
                    return;

                Unset(view);
            }

            view.HasModel = true;
            view.Model = model;
            OnModelSet(view, model);
        }

        public void Unset(TView view)
        {
            if (!view.HasModel)
                return;

            view.HasModel = false;
            foreach (var child in view)
                UiBootstrap.Instance.Unset(child);

            OnModelUnset(view);
        }

        protected abstract void OnModelSet(TView view, TModel model);

        protected virtual void OnModelUnset(TView view)
        {
        }

        public void Unset(IView view) => Unset((TView) view);

        public bool AcceptableView(Type viewType)
            => viewType == typeof(TView) || typeof(TView).IsAssignableFrom(viewType);
    }
}