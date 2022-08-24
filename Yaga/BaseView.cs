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
        public bool IsOpened { protected set; get; }
        bool IView.IsSetted { get; set; }

        public virtual void Open()
        {
            if (IsOpened)
                return;

            IsOpened = true;
            gameObject.SetActive(true);
        }

        public virtual void Close()
        {
            if (!IsOpened)
                return;

            IsOpened = false;
            gameObject.SetActive(false);
        }

        public virtual IView Create(Transform parent)
        {
            var result = Instantiate(this, parent);
            result.gameObject.SetActive(false);
            return result;
        }

        public virtual void Destroy() => Destroy(gameObject);

        public void OnDestroy()
        {
            if (((IView)this).IsSetted)
                UiBootstrap.Instance.Unset(this);
        }
        
        internal readonly List<IDisposable> Disposables = new List<IDisposable>();

        public virtual void Dispose()
        {
            foreach (var disposable in Disposables) disposable.Dispose();
            Disposables.Clear();
        }

        public bool Equals(IView other) => other != null && other.GetInstanceID() == GetInstanceID();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public IEnumerator<IView> GetEnumerator() => Children.Cast<IView>().GetEnumerator();
        public override string ToString() => gameObject.name;
        
        /// <summary>
        /// Subscribe on observable value change and dispose subscription when needed.
        /// </summary>
        public void Subscribe<T>(Utils.IObservable<T> observable, Action<T> action)
            => Disposables.Add(observable.Subscribe(action));

        /// <summary>
        /// Subscribe on observable value change and dispose subscription when needed.
        /// </summary>
        public void Subscribe<T>(IOptionalObservable<T> observable, Action<T> action, Action onNull)
            => Disposables.Add(observable.Subscribe(action, onNull));

        /// <summary>
        /// Subscribe on beacon and dispose subscription when needed.
        /// </summary>
        public void Subscribe(Beacon observable, Action action)
            => Disposables.Add(observable.Add(action));

        /// <summary>
        /// Subscribe on UnityEvent and dispose subscription when needed.
        /// </summary>
        public void Subscribe(UnityEvent @event, UnityAction action)
        {
            @event.AddListener(action);
            Disposables.Add(new Reflector(() => @event.RemoveListener(action)));
        }

        /// <summary>
        /// Subscribe on beacon and dispose subscription when needed.
        /// </summary>
        public void Subscribe<T1, T2>(Beacon<T1, T2> observable, Action<T1, T2> action)
            => Disposables.Add(observable.Add(action));

        /// <summary>
        /// Subscribe on beacon and dispose subscription when needed.
        /// </summary>
        public void Subscribe<T1, T2, T3>(Beacon<T1, T2, T3> observable, Action<T1, T2, T3> action)
            => Disposables.Add(observable.Add(action));

        /// <summary>
        /// Subscribe on beacon and dispose subscription when needed.
        /// </summary>
        public void Subscribe<T>(Beacon<T> observable, Action<T> action)
            => Disposables.Add(observable.Add(action));

        /// <summary>
        /// Subscribe on observable value change, execute action and dispose subscription when needed.
        /// </summary>
        public void SubscribeAndCall<T>(Utils.IObservable<T> observable, Action<T> action)
        {
            Disposables.Add(observable.Subscribe(action));
            action(observable.Data);
        }

        /// <summary>
        /// Subscribe on observable value change and dispose subscription when needed.
        /// </summary>
        public void SubscribeAndCall<T>(IOptionalObservable<T> observable, Action<T> action, Action onNull)
        {
            Disposables.Add(observable.Subscribe(action, onNull));
            if (observable.IsDefault)
                onNull();
            else
                action(observable.Data);
        }

        public void AddDisposable(IDisposable disposable) => Disposables.Add(disposable);
    }
}