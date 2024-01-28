using System;
using TMPro;
using UnityEngine.UI;

namespace Yaga.Binding.Observable.ConcreteObservables
{
    public class BindStringObservable : BindObservable<string>
    {
        public BindStringObservable(
            BindingContext context,
            Utils.IReadOnlyObservable<string> observable,
            Action onDispose = default
        ) : base(context, observable, onDispose)
        {
        }

        public IBindAccessor To(TextMeshProUGUI view)
        {
            var accessor = new BindAccessor(() => { view.SetText(Data); }, OnDispose);
            Context._bindings.Add(accessor);
            return accessor;
        }
        
        public IBindAccessor To(Text view)
        {
            var accessor = new BindAccessor(() => { view.text = Data; }, OnDispose);
            Context._bindings.Add(accessor);
            return accessor;
        }
    }
}