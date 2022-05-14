using System;
using System.Collections.Generic;
using System.Linq;
using Yaga.Utils;

namespace Yaga
{
    /// <summary>
    /// Responsible of UI flow.
    /// </summary>
    public class UiBootstrap
    {
        public static readonly UiBootstrap Instance = new UiBootstrap();

        /// <summary>
        /// List of all bound presenters.
        /// </summary>
        private List<IPresenter> _presenters = new List<IPresenter>();

        /// <summary>
        /// Binds presenter to view. Remember that presenter and view has to has single-to-single relation.
        /// </summary>
        public static void Bind(IPresenter presenter)
        {
            Instance._presenters.Add(presenter);
        }

        /// <summary>
        /// Binds presenter with empty constructor to view. Remember that presenter and view has to has single-to-single relation.
        /// </summary>
        public static void Bind<TPresenter>()
            where TPresenter : IPresenter
        {
            Instance._presenters.Add(Activator.CreateInstance<TPresenter>());
        }

        /// <summary>
        /// Method for reflection for bindings. Don't use, don't rename, don't delete.
        /// </summary>
        private void SetView<TView, TModel>(TView view, TModel model)
            where TView : IView<TModel>
            => Set(view, model);

        /// <summary>
        /// Sets model to provided view.
        /// </summary>
        /// <exception cref="PresenterNotFoundException">If there are zero or more than one acceptable presenter for view.</exception>
        public void Set<TView, TModel>(TView view, TModel model)
            where TView : IView<TModel>
        {
            if (view.HasModel && view.Model.Equals(model))
                return;

            var controller = (IPresenter<TView, TModel>) GetController(view.GetType());
            controller.Set(view, model);
        }

        /// <summary>
        /// Method for reflection for bindings. Don't use, don't rename, don't delete.
        /// </summary>
        private void SetList<TChild, TModel>(ListView<TChild, TModel> view, IEnumerable<TModel> model)
            where TChild : BaseView<TModel>
            => Set(view, model);

        /// <summary>
        /// Sets model to provided view.
        /// </summary>
        /// <exception cref="PresenterNotFoundException">If there are zero or more than one acceptable presenter for view.</exception>
        public void Set<TChild, TModel>(ListView<TChild, TModel> view, IEnumerable<TModel> model)
            where TChild : BaseView<TModel>
        {
            if (view.HasModel && view.Model.Equals(model))
                return;

            var controller
                = (IPresenter<ListView<TChild, TModel>, IObservableEnumerable<TModel>>) GetController(view.GetType());
            controller.Set(view, new ObservableEnumerable<TModel>(model));
        }

        /// <summary>
        /// Sets model to provided view.
        /// </summary>
        /// <exception cref="PresenterNotFoundException">If there are zero or more than one acceptable presenter for view.</exception>
        public void Set<TChild, TModel>(ListView<TChild, TModel> view, IObservableEnumerable<TModel> model)
            where TChild : BaseView<TModel>
        {
            if (view.HasModel && view.Model.Equals(model))
                return;

            var controller
                = (IPresenter<ListView<TChild, TModel>, IObservableEnumerable<TModel>>) GetController(view.GetType());
            controller.Set(view, model);
        }

        /// <summary>
        /// Calls unset method for certain view.
        /// </summary>
        /// <exception cref="PresenterNotFoundException">If there are zero or more than one acceptable presenter for view.</exception>
        public void Unset(IView view) => GetController(view.GetType()).Unset(view);

        private IPresenter GetController(Type viewType)
        {
            try
            {
                var controller = _presenters.SingleOrDefault(e => e.AcceptableView(viewType));
                if (controller == default)
                    throw new PresenterNotFoundException(viewType);
                return controller;
            }
            catch (InvalidOperationException)
            {
                throw new MultiplePresenterException(viewType);
            }
        }
    }
}