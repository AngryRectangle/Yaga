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

        public virtual void Create()
        {
            IsInstanced = true;
            gameObject.SetActive(false);
        }

        public virtual void Destroy()
        {
            Destroy(gameObject);
        }

        public void OnDestroy()
        {
            if (((IView)this).IsSetted)
                UiBootstrap.Instance.Unset(this);
        }

        public bool Equals(IView other) => other != null && other.GetInstanceID() == GetInstanceID();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public IEnumerator<IView> GetEnumerator() => Children.Cast<IView>().GetEnumerator();
        public override string ToString() => gameObject.name;
    }
}