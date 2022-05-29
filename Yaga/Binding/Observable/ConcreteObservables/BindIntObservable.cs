using System;
using TMPro;
using UnityEngine.UI;

namespace Yaga.Binding.Observable.ConcreteObservables
{
    public class BindIntObservable : BindObservable<int>
    {
        public BindIntObservable(
            BindingContext context,
            Utils.IObservable<int> observable,
            Action onDispose = default) : base(context, observable, onDispose)
        {
        }

        public IBindAccessor To(TextMeshProUGUI view, IFormatProvider formatProvider)
        {
            var accessor = new BindAccessor(() => { view.SetText(Data.ToString(formatProvider)); }, OnDispose);
            Context._bindings.Add(accessor);
            return accessor;
        }

        public IBindAccessor To(TextMeshProUGUI view, string format)
        {
            var accessor = new BindAccessor(() => { view.SetText(Data.ToString(format)); }, OnDispose);
            Context._bindings.Add(accessor);
            return accessor;
        }

        public IBindAccessor To(TextMeshProUGUI view, string format, IFormatProvider formatProvider)
        {
            var accessor = new BindAccessor(() => { view.SetText(Data.ToString(format, formatProvider)); }, OnDispose);
            Context._bindings.Add(accessor);
            return accessor;
        }

        public IBindAccessor To(TextMeshProUGUI view)
        {
            var accessor = new BindAccessor(() => { view.SetText(Data.ToString()); }, OnDispose);
            Context._bindings.Add(accessor);
            return accessor;
        }

        public IBindAccessor To(Text view)
        {
            var accessor = new BindAccessor(() => { view.text = Data.ToString(); }, OnDispose);
            Context._bindings.Add(accessor);
            return accessor;
        }

        public IBindAccessor To(Text view, IFormatProvider formatProvider)
        {
            var accessor = new BindAccessor(() => { view.text = Data.ToString(formatProvider); }, OnDispose);
            Context._bindings.Add(accessor);
            return accessor;
        }

        public IBindAccessor To(Text view, string format)
        {
            var accessor = new BindAccessor(() => { view.text = Data.ToString(format); }, OnDispose);
            Context._bindings.Add(accessor);
            return accessor;
        }

        public IBindAccessor To(Text view, string format, IFormatProvider formatProvider)
        {
            var accessor = new BindAccessor(() => { view.text = Data.ToString(format, formatProvider); }, OnDispose);
            Context._bindings.Add(accessor);
            return accessor;
        }
    }
}