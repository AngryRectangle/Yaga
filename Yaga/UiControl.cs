using System;
using UnityEngine;
using Yaga.Exceptions;

namespace Yaga
{
    /// <summary>
    /// Contains shortcut of many operation with UI.
    /// </summary>
    public class UiControl
    {
        private readonly Canvas _canvasPrefab;
        private static UiControl _instance;

        /// <summary>
        /// Singleton instance of <see cref="UiControl"/>. Requires to be installed before using.
        /// </summary>
        /// <exception cref="UiControlInitializationException">If singleton wasn't initialized.</exception>
        public static UiControl Instance
        {
            get => _instance ?? throw new UiControlInitializationException();
            internal set => _instance = value;
        }

        private UiControl(Canvas canvasPrefab)
        {
            _canvasPrefab = canvasPrefab;
        }

        /// <summary>
        /// It is necessary method for <see cref="UiControl"/>> initialization.
        /// <exception cref="NullReferenceException">If canvasPrefab is null</exception>
        /// </summary>
        public static void InitializeSingleton(Canvas canvasPrefab)
        {
            if (canvasPrefab is null)
                throw new NullReferenceException(nameof(canvasPrefab));

            Instance = new UiControl(canvasPrefab);
        }

        /// <inheritdoc cref="UiControl.Create{TView, TModel}(TView, TModel, Transform)"/>
        /// <remarks>Parent for view is created from <see cref="UiControl"/> Canvas</remarks>
        public ViewControl<TView, TModel> Create<TView, TModel>(TView prefab, TModel model)
            where TView : IView<TModel>
        {
            var canvas = MonoBehaviour.Instantiate(_canvasPrefab);
            return Create(prefab, model, (RectTransform)canvas.transform, true);
        }
        
        /// <inheritdoc cref="UiControl.Create{TView}(TView, Transform)"/>
        /// <remarks>Parent for view is created from <see cref="UiControl"/> Canvas</remarks>
        public ViewControl<TView, Unit> Create<TView>(TView prefab)
            where TView : IView<Unit>
        {
            var canvas = MonoBehaviour.Instantiate(_canvasPrefab);
            return Create(prefab, Unit.Instance, (RectTransform)canvas.transform, true);
        }

        /// <inheritdoc cref="UiControl.Create{TView, Unit}(TView, Transform)"/>
        public ViewControl<TView, Unit> Create<TView>(TView prefab, RectTransform parent,
            bool rootParent = false)
            where TView : IView<Unit>
        {
            return Create(prefab, Unit.Instance, parent, rootParent);
        }

        /// <summary>
        /// Shortcut to create view. Instantiates view and sets parent for it.
        /// Calls <see cref="IView.Create"/> on view,
        /// <see cref="UiBootstrap.Set{TView, TModel}(TView, TModel)"/>
        /// and <see cref="IView.Open"/> under hood.
        /// It is recommended to use this method if you are outside of presenter's code.
        /// </summary>
        /// <param name="parent">Transform of object that will be parent for view</param>
        /// <param name="rootParent">If true, provided parent transform will be destroyed when view will</param>
        /// <exception cref="IsNotPrefabException">If provided prefab was not an actual prefab.</exception>
        /// <exception cref="ArgumentNullException">If prefab, parent or model is null.</exception>
        /// <inheritdoc cref="UiBootstrap.Set{TView, TModel}(TView, TModel)"/>
        public ViewControl<TView, TModel> Create<TView, TModel>(TView prefab, TModel model, RectTransform parent,
            bool rootParent = false)
            where TView : IView<TModel>
        {
            if (prefab is null)
                throw new ArgumentNullException(nameof(prefab));
            if (model is null)
                throw new ArgumentNullException(nameof(model));
            if (parent is null)
                throw new ArgumentNullException(nameof(parent));

            if (!prefab.IsPrefab)
                throw new IsNotPrefabException(prefab);

            var instance = (TView)prefab.Create(parent, rootParent);
            var subs = UiBootstrap.Instance.Set(instance, model);
            return new ViewControl<TView, TModel>(instance, subs);
        }
    }
}