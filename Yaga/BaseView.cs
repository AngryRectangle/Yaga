using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Yaga.Utils;

namespace Yaga
{
    public abstract class BaseView : MonoBehaviour, IView
    {
        public abstract IEnumerable<IView> Children { get; }
        public bool IsPrefab => gameObject.scene.name is null;
        bool IView.IsSetted { get; set; }
        private Transform _rootParent;

        public virtual IView Create(Transform parent)
        {
            var result = Instantiate(this, parent);
            result.gameObject.SetActive(false);
            return result;
        }

        public virtual void Destroy()
        {
            Destroy(_rootParent is null ? gameObject : _rootParent.gameObject);
        }

        public void OnDestroy()
        {
            if (((IView)this).IsSetted)
                UiBootstrap.Instance.Unset(this);
        }

        private readonly List<Action> _onUnsubscriptionActions = new List<Action>();

        void IView.OnUnsubscribe()
        {
            OnUnsubscribe();
        }

        void IView.SetAsRootParent(Transform parent)
        {
            _rootParent = parent;
        }

        protected virtual void OnUnsubscribe()
        {
            foreach (var disposable in _onUnsubscriptionActions) disposable();
            _onUnsubscriptionActions.Clear();
        }

        public void AddUnsubscription(IDisposable disposable) => _onUnsubscriptionActions.Add(disposable.Dispose);
        public void AddUnsubscription(Action onDispose) => _onUnsubscriptionActions.Add(onDispose);

        public bool Equals(IView other) => other != null && other.GetInstanceID() == GetInstanceID();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public IEnumerator<IView> GetEnumerator() => Children.Cast<IView>().GetEnumerator();
        public override string ToString() => gameObject.name;

        /// <summary>
        /// Subscribe on observable value change and dispose subscription when needed.
        /// </summary>
        public void Subscribe<T>(Utils.IObservable<T> observable, Action<T> action)
            => AddUnsubscription(observable.Subscribe(action));

        /// <summary>
        /// Subscribe on observable value change and dispose subscription when needed.
        /// </summary>
        public void Subscribe<T>(IOptionalObservable<T> observable, Action<T> action, Action onNull)
            => AddUnsubscription(observable.Subscribe(action, onNull));

        /// <summary>
        /// Subscribe on beacon and dispose subscription when needed.
        /// </summary>
        public void Subscribe(Beacon observable, Action action)
            => AddUnsubscription(observable.Add(action));

        /// <summary>
        /// Subscribe on UnityEvent and dispose subscription when needed.
        /// </summary>
        public void Subscribe(UnityEvent @event, UnityAction action)
        {
            @event.AddListener(action);
            AddUnsubscription(() => @event.RemoveListener(action));
        }

        /// <summary>
        /// Subscribe on beacon and dispose subscription when needed.
        /// </summary>
        public void Subscribe<T1, T2>(Beacon<T1, T2> observable, Action<T1, T2> action)
            => AddUnsubscription(observable.Add(action));

        /// <summary>
        /// Subscribe on beacon and dispose subscription when needed.
        /// </summary>
        public void Subscribe<T1, T2, T3>(Beacon<T1, T2, T3> observable, Action<T1, T2, T3> action)
            => AddUnsubscription(observable.Add(action));

        /// <summary>
        /// Subscribe on beacon and dispose subscription when needed.
        /// </summary>
        public void Subscribe<T>(Beacon<T> observable, Action<T> action)
            => AddUnsubscription(observable.Add(action));

        /// <summary>
        /// Subscribe on observable value change, execute action and dispose subscription when needed.
        /// </summary>
        public void SubscribeAndCall<T>(Utils.IObservable<T> observable, Action<T> action)
        {
            AddUnsubscription(observable.Subscribe(action));
            action(observable.Data);
        }

        /// <summary>
        /// Subscribe on observable value change and dispose subscription when needed.
        /// </summary>
        public void SubscribeAndCall<T>(IOptionalObservable<T> observable, Action<T> action, Action onNull)
        {
            AddUnsubscription(observable.Subscribe(action, onNull));
            observable.Data.Match(action, onNull);
        }
    }
}