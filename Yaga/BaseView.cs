using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Yaga
{
    public abstract class BaseView : MonoBehaviour, IView
    {
        public abstract IEnumerable<IView> Children { get; }
        public bool IsOpened { protected set; get; }
        public bool IsInstanced { protected set; get; }

        public virtual void Open()
        {
            if (IsOpened)
                return;

            IsOpened = true;
            gameObject.SetActive(true);
            #if UBER_LOG
            UberDebug.LogChannel(Channel.UI, $"Opened UI: {gameObject.name}");
            #endif
        }

        public virtual void Close()
        {
            if (!IsOpened)
                return;

            IsOpened = false;
            gameObject.SetActive(false);
            #if UBER_LOG
            UberDebug.LogChannel(Channel.UI, $"Closed UI: {gameObject.name}");
            #endif
        }

        public virtual void Create()
        {
            IsInstanced = true;
            gameObject.SetActive(false);
            #if UBER_LOG
            UberDebug.LogChannel(Channel.UI, $"Created UI: {gameObject.name}");
            #endif
        }

        public virtual void Destroy()
        {
            Destroy(gameObject);
            #if UBER_LOG
            UberDebug.LogChannel(Channel.UI, $"Destroyed UI: {gameObject.name}");
            #endif
        }

        public bool Equals(IView other) => other != null && other.GetInstanceID() == GetInstanceID();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public IEnumerator<IView> GetEnumerator() => Children.Cast<IView>().GetEnumerator();
        public override string ToString() => gameObject.name;
    }
}