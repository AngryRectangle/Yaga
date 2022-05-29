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
        bool AcceptableView(Type viewType);
        void Unset(IView view);
    }

    public interface IPresenter<in TView> : IPresenter
        where TView : IView
    {
        void Set(TView view);
        void Unset(TView view);
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

        public void Unset(IView view) => Unset((TView)view);

        public bool AcceptableView(Type viewType)
            => viewType == typeof(TView) || typeof(TView).IsAssignableFrom(viewType);
    }

    public abstract class Presenter<TView> : IPresenter<TView>
        where TView : View
    {
        public void Set(IView view) => Set((TView)view);
        public void Set(TView view) => OnSet(view);

        protected virtual void OnSet(TView view)
        {
        }

        public void Unset(IView view) => Unset((TView)view);

        public void Unset(TView view)
        {
            foreach (var child in view)
                UiBootstrap.Instance.Unset(child);

            OnUnset(view);
        }

        protected virtual void OnUnset(TView view)
        {
        }

        public bool AcceptableView(Type viewType)
            => viewType == typeof(TView) || typeof(TView).IsAssignableFrom(viewType);
    }
}