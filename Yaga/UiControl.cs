using UnityEngine;

namespace Yaga
{
    /// <summary>
    /// Contains shortcut of many operation with UI.
    /// </summary>
    public class UiControl
    {
        private readonly Canvas _canvasPrefab;

        /// <summary>
        /// Null if <see cref="UiControl"/> was not initialized.
        /// </summary>
        public static UiControl Instance { private set; get; }

        private UiControl(Canvas canvasPrefab)
        {
            _canvasPrefab = canvasPrefab;
        }

        /// <summary>
        /// It is necessary method for <see cref="UiControl"/>> initialization.
        /// </summary>
        public static void InitializeSingleton(Canvas canvasPrefab)
        {
            Instance = new UiControl(canvasPrefab);
        }

        /// <summary>
        /// Shortcut to create view. Instantiates canvas for view instance and make it view parent.
        /// Calls <see cref="UiBootstrap.Create"/>,
        /// <see cref="UiBootstrap.Set"/>, <see cref="UiBootstrap.Open"/> underhood.
        /// It is recommended to use this method if you are outside of presenter's code.
        /// </summary>
        public TView Create<TView, TModel>(TView prefab, TModel model)
            where TView : MonoBehaviour, IView<TModel>
        {
            var canvas = MonoBehaviour.Instantiate(_canvasPrefab);
            return Create(prefab, model, canvas.transform);
        }

        /// <summary>
        /// Shortcut to create view. Instantiates view and sets parent for it.
        /// Calls <see cref="UiBootstrap.Create"/>,
        /// <see cref="UiBootstrap.Set"/>, <see cref="UiBootstrap.Open"/> underhood.
        /// It is recommended to use this method if you are outside of presenter's code.
        /// </summary>
        public TView Create<TView, TModel>(TView prefab, TModel model, Transform parent)
            where TView : MonoBehaviour, IView<TModel>
        {
            if (prefab.gameObject.scene.isLoaded)
                Debug.LogWarning("Provided game object is not prefab");

            var instance = MonoBehaviour.Instantiate(prefab, parent);
            instance.Create();
            UiBootstrap.Instance.Set(instance, model);
            instance.Open();
            return instance;
        }

        /// <summary>
        /// Shortcut to create view. Instantiates canvas for view instance and make it view parent.
        /// Calls <see cref="UiBootstrap.Create"/>,
        /// <see cref="UiBootstrap.Set"/>, <see cref="UiBootstrap.Open"/> underhood.
        /// It is recommended to use this method if you are outside of presenter's code.
        /// </summary>
        public TView Create<TView>(TView prefab)
            where TView : MonoBehaviour, IView
        {
            var canvas = MonoBehaviour.Instantiate(_canvasPrefab);
            return Create(prefab, canvas.transform);
        }

        /// <summary>
        /// Shortcut to create view. Instantiates view and sets parent for it.
        /// Calls <see cref="UiBootstrap.Create"/>,
        /// <see cref="UiBootstrap.Set"/>, <see cref="UiBootstrap.Open"/> underhood.
        /// It is recommended to use this method if you are outside of presenter's code.
        /// </summary>
        public TView Create<TView>(TView prefab, Transform parent)
            where TView : MonoBehaviour, IView
        {
            if (prefab.gameObject.scene.isLoaded)
                Debug.LogWarning("Provided game object is not prefab");

            var instance = MonoBehaviour.Instantiate(prefab, parent);
            instance.Create();
            var presenter = UiBootstrap.Instance.GetController(typeof(TView));
            ((IPresenter<TView>)presenter).Set(instance);
            instance.Open();
            return instance;
        }

        /// <summary>
        /// Shortcut to destroy view. Calls <see cref="UiBootstrap.Close"/>,
        /// <see cref="UiBootstrap.Unset"/>, <see cref="UiBootstrap.Destroy"/> underhood.
        /// It is recommended to use this method if you are outside of presenter's code.
        /// </summary>
        public void Destroy<TView>(TView view)
            where TView : IView
        {
            view.Close();
            UiBootstrap.Instance.Unset(view);
            view.Destroy();
        }
    }
}