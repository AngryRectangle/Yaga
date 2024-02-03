using System;
using System.Collections.Generic;
using System.Linq;
using Yaga.Exceptions;

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
        private readonly Dictionary<Type, IPresenter> _presenters;

        private UiBootstrap()
        {
            _presenters = new Dictionary<Type, IPresenter>();
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
        /// Binds presenter to view. Remember that presenter and view have to have single-to-single relation.
        /// </summary>
        /// <exception cref="ArgumentNullException">If presenter is null</exception>
        /// <exception cref="MultiplePresenterException">If there are more then one acceptable <see cref="IPresenter"/> for view.</exception>
        /// <inheritdoc cref="GetAcceptableViewTypes"/>
        public void Bind(IPresenter presenter)
        {
            if (presenter == null)
                throw new ArgumentNullException(nameof(presenter));

            var acceptableViews = GetAcceptableViewTypes(presenter);
            foreach (var viewType in acceptableViews)
            {
                if (_presenters.TryGetValue(viewType, out var existingPresenter))
                    throw new MultiplePresenterException(viewType, existingPresenter.GetType(), presenter.GetType());

                _presenters.Add(viewType, presenter);
            }
        }

        /// <summary>
        /// Binds presenter with default constructor to view. Remember that presenter and view have to have single-to-single relation.
        /// </summary>
        /// <exception cref="NoDefaultConstructorForPresenterException">If presenter has no default constructor.</exception>
        /// <exception cref="MultiplePresenterException">If there are more then one acceptable <see cref="IPresenter"/> for view.</exception>
        /// <inheritdoc cref="GetAcceptableViewTypes"/>
        public void Bind<TPresenter>()
            where TPresenter : IPresenter
        {
            try
            {
                var instance = Activator.CreateInstance<TPresenter>();
                Bind(instance);
            }
            catch (MissingMethodException _)
            {
                throw new NoDefaultConstructorForPresenterException(typeof(TPresenter),
                    "This method supports only presenters with default constructor. Try use another overload or create parameterless constructor.");
            }
        }

        /// <summary>
        /// Clear all bound presenters.
        /// </summary>
        /// <inheritdoc cref="Instance"/>
        public void ClearPresenters()
        {
            Instance._presenters.Clear();
        }

        /// <summary>
        /// Sets model to provided view.
        /// </summary>
        /// <exception cref="ArgumentNullException">If view or model is null.</exception>
        /// <inheritdoc cref="UiBootstrap.GetController"/>
        internal Subscriptions Set<TView, TModel>(TView view, TModel model)
            where TView : IView<TModel>
        {
            if (view is null)
                throw new ArgumentNullException(nameof(view));

            if (model is null)
                throw new ArgumentNullException(nameof(model));

            /*
             * Previously there was this line of code:
             * var controller = (IPresenter<TView, TModel>)GetController(view.GetType());
             * But due to c# generics stupidity it's not possible to cast presenter to (IPresenter<TView, TModel>)
             * because you can call this method on type "View<string>" which would be correct,
             * but it leads to InvalidCastException.
             * So to fix it, I've created IPresenterWithUnspecifiedView interface
             * to avoid reflection and boilerplate with Set method call.
             */

            if (_presenters.TryGetValue(view.GetType(), out var presenter))
                return presenter.Set(view, model);

            throw new PresenterNotFoundException(view.GetType());
        }

        /// <exception cref="PresenterBindingException">if presenter doesn't implement <see cref="IPresenter{TView,TModel}"/>
        /// interface or implements more then one.</exception>
        private static IEnumerable<Type> GetAcceptableViewTypes(IPresenter presenter)
        {
            var presenterType = presenter.GetType();
            var interfaces = presenterType.GetInterfaces();
            var viewTypesEnumerable = interfaces
                .Where(e => e.IsGenericType && e.GetGenericTypeDefinition() == typeof(IPresenter<,>))
                .Select(e => e.GetGenericArguments()[0]);

            Type viewType;
            try
            {
                viewType = viewTypesEnumerable.SingleOrDefault();
            }
            catch (InvalidOperationException _)
            {
                throw new PresenterBindingException("Presenter implements more then one IPresenter<,> interface.",
                    presenterType);
            }

            if (viewType is null)
                throw new PresenterBindingException("Presenter doesn't implement IPresenter<,> interface.",
                    presenterType);

            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.DefinedTypes)
                .Where(type => viewType.IsAssignableFrom(type))
                .Select(e => e.AsType());
        }
    }
}