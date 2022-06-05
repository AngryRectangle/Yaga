using System;
using System.Collections.Generic;
using System.Linq;
using Yaga.Exceptions;
using Yaga.Utils;

namespace Yaga
{
    /// <summary>
    /// Responsible of UI flow.
    /// </summary>
    public class UiBootstrap
    {
        private static UiBootstrap _instance;

        /// <summary>
        /// Singleton instance of <see cref="UiBootstrap"/>. Requires to be installed before using.
        /// </summary>
        /// <exception cref="UiBootstrapInitializationException">If singleton wasn't initialized.</exception>
        public static UiBootstrap Instance
        {
            get => _instance ?? throw new UiBootstrapInitializationException();
            private set => _instance = value;
        }

        /// <summary>
        /// List of all bound presenters.
        /// </summary>
        private List<IPresenter> _presenters;

        private UiBootstrap()
        {
            _presenters = new List<IPresenter>();
        }

        public UiBootstrap(List<IPresenter> presenters)
        {
            _presenters = presenters ?? throw new ArgumentNullException(nameof(presenters));
        }
        
        /// <summary>
        /// Set instance of <see cref="UiBootstrap"/> to <see cref="Instance"/> property.
        /// </summary>
        /// <exception cref="ArgumentNullException">If bootstrap is null.</exception>
        public static void InitializeSingleton(UiBootstrap bootstrap)
        {
            Instance = bootstrap ?? throw new ArgumentNullException(nameof(bootstrap));
        }

        /// <summary>
        /// Set instance of <see cref="UiBootstrap"/> to <see cref="Instance"/> property.
        /// </summary>
        public static void InitializeSingleton()
        {
            Instance = new UiBootstrap();
        }

        /// <summary>
        /// Binds presenter to view. Remember that presenter and view has to has single-to-single relation.
        /// <exception cref="ArgumentNullException">If presenter is null</exception>
        /// </summary>
        /// <inheritdoc cref="Instance"/>
        public static void Bind(IPresenter presenter)
        {
            Instance._presenters.Add(presenter ?? throw new ArgumentNullException(nameof(presenter)));
        }

        /// <summary>
        /// Binds presenter with default constructor to view. Remember that presenter and view has to has single-to-single relation.
        /// </summary>
        /// <exception cref="NoDefaultConstructorForPresenterException">If presenter has no default constructor.</exception>
        /// <inheritdoc cref="Instance"/>
        public static void Bind<TPresenter>()
            where TPresenter : IPresenter
        {
            try
            {
                var instance = Activator.CreateInstance<TPresenter>();
                Instance._presenters.Add(instance);
            }
            catch (MissingMethodException _)
            {
                throw new NoDefaultConstructorForPresenterException(typeof(TPresenter),
                    "This method supports only presenters with default constructor. Try use another overload or create parameterless constructor.");
            }
        }

        /// <summary>
        /// Clear all binded presenters.
        /// </summary>
        /// <inheritdoc cref="Instance"/>
        public static void ClearPresenters()
        {
            Instance._presenters.Clear();
        }

        /// <summary>
        /// Sets model to provided view.
        /// </summary>
        /// <exception cref="ArgumentNullException">If view or model is null.</exception>
        /// <inheritdoc cref="UiBootstrap.GetController"/>
        public void Set<TView, TModel>(TView view, TModel model)
            where TView : IView<TModel>
        {
            if (view is null)
                throw new ArgumentNullException(nameof(view));

            if (model is null)
                throw new ArgumentNullException(nameof(model));

            if (view.HasModel && view.Model.Equals(model))
                return;

            var controller = (IPresenter<TView, TModel>)GetController(view.GetType());
            controller.Set(view, model);
        }

        /// <summary>
        /// Call set method for presenter for view without model.
        /// </summary>
        /// <exception cref="ArgumentNullException">If view is null.</exception>
        /// <exception cref="ArgumentException">If TView requires model</exception>
        /// <inheritdoc cref="UiBootstrap.GetController"/>
        public void Set<TView>(TView view)
            where TView : IView
        {
            if (view is null)
                throw new ArgumentNullException(nameof(view));

            var controller = GetController(view.GetType()) as IPresenter<TView>;
            if (controller is null)
                throw new ArgumentException($"{typeof(TView)} require model", nameof(view));

            controller.Set(view);
        }

        /// <summary>
        /// Sets model to provided view.
        /// </summary>
        /// <exception cref="ArgumentNullException">If view or model is null.</exception>
        /// <inheritdoc cref="UiBootstrap.GetController"/>
        public void Set<TChild, TModel>(ListView<TChild, TModel> view, IEnumerable<TModel> model)
            where TChild : View<TModel>
        {
            if (view is null)
                throw new ArgumentNullException(nameof(view));

            if (model is null)
                throw new ArgumentNullException(nameof(model));

            if (view.HasModel && view.Model.Equals(model))
                return;

            var controller
                = (IPresenter<ListView<TChild, TModel>, IObservableEnumerable<TModel>>)GetController(view.GetType());
            controller.Set(view, new ObservableEnumerable<TModel>(model));
        }

        /// <summary>
        /// Sets model to provided view.
        /// </summary>
        /// <exception cref="ArgumentNullException">If view or model is null.</exception>
        /// <inheritdoc cref="UiBootstrap.GetController"/>
        public void Set<TChild, TModel>(ListView<TChild, TModel> view, IObservableEnumerable<TModel> model)
            where TChild : View<TModel>
        {
            if (view is null)
                throw new ArgumentNullException(nameof(view));

            if (model is null)
                throw new ArgumentNullException(nameof(model));

            if (view.HasModel && view.Model.Equals(model))
                return;

            var controller
                = (IPresenter<ListView<TChild, TModel>, IObservableEnumerable<TModel>>)GetController(view.GetType());
            controller.Set(view, model);
        }

        /// <summary>
        /// Calls unset method for certain view.
        /// </summary>
        /// <exception cref="ArgumentNullException">If view is null.</exception>
        /// <inheritdoc cref="UiBootstrap.GetController"/>
        public void Unset(IView view)
        {
            if (view is null)
                throw new ArgumentNullException(nameof(view));

            GetController(view.GetType()).Unset(view);
        }

        /// <summary>
        /// Get acceptable presenter from presenters list for view.
        /// </summary>
        /// <exception cref="PresenterNotFoundException">If there is no acceptable presenter for view.</exception>
        /// <exception cref="MultiplePresenterException">If there are more than one acceptable presenter for view.</exception>
        internal IPresenter GetController(Type viewType)
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

        /// <summary>
        /// Method for reflection for bindings. Don't use, don't rename, don't delete.
        /// </summary>
        private void SetView<TView, TModel>(TView view, TModel model)
            where TView : IView<TModel>
            => Set(view, model);

        /// <summary>
        /// Method for reflection for bindings. Don't use, don't rename, don't delete.
        /// </summary>
        private void SetList<TChild, TModel>(ListView<TChild, TModel> view, IEnumerable<TModel> model)
            where TChild : View<TModel>
            => Set(view, model);
    }
}