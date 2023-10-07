using System;
using System.Collections.Generic;
using UnityEngine;
using Yaga.Utils;

namespace Yaga.Components
{
    public class RadioGroup<T> : View<RadioGroup<T>.RadioGroupModel>
    {
        [SerializeField] private RadioButton<T> prefab;
        private List<RadioButton<T>> _buttons;
        private RadioButton<T> _lastSelected;
        public override IEnumerable<IView> Children => _buttons;

        public class Presenter : Presenter<RadioGroup<T>, RadioGroupModel>
        {
            protected override void OnModelSet(RadioGroup<T> view, RadioGroupModel model)
            {
                view.Subscribe(model.Collection.ItemAdded,
                    (index, value) => CreateButton(view, model.Collection.GetObservable(index), index));

                view.Subscribe(model.Collection.ItemRemoved, (index, value) =>
                {
                    var targetButton = view._buttons[index];
                    view._buttons.Remove(targetButton);
                    if (view._lastSelected == targetButton)
                        view.Model.SelectedElement.SetDefault();

                    UiControl.Instance.Destroy(targetButton);
                });

                for (var i = 0; i < model.Collection.Count; i++)
                    CreateButton(view, model.Collection.GetObservable(i));

                view.SubscribeAndCall(model.SelectedElement, selected =>
                {
                    if (view._lastSelected)
                        view._lastSelected.Model.Selected.Data = false;

                    view._lastSelected = selected;
                    selected.Model.Selected.Data = true;
                }, () =>
                {
                    if (view._lastSelected)
                        view._lastSelected.Model.Selected.Data = false;

                    view._lastSelected = null;
                });
            }

            private void CreateButton(RadioGroup<T> view, IOptionalObservable<T> model, int index = -1)
            {
                var buttonModel = new RadioButton<T>.RadioButtonModel(false, model);
                var radioButton = UiControl.Instance.Create(view.prefab, buttonModel, view.transform);
                radioButton.Subscribe(radioButton.OnClick, () => view.Model.SelectedElement.Data = radioButton);
                if (index < 0)
                {
                    view._buttons.Add(radioButton);
                    return;
                }

                view._buttons.Insert(index, radioButton);
                radioButton.transform.SetSiblingIndex(index);
            }
        }

        public readonly struct RadioGroupModel
        {
            public readonly IObservableList<T> Collection;
            public readonly OptionalObservable<RadioButton<T>> SelectedElement;
            public readonly IOptionalObservable<T> SelectedValue;

            public RadioGroupModel(IObservableList<T> collection, OptionalObservable<RadioButton<T>> selectedElement)
            {
                Collection = collection;
                SelectedElement = selectedElement;
                SelectedValue = new RadioGroupObservable(selectedElement);
            }

            private class RadioGroupObservable : IOptionalObservable<T>
            {
                private readonly OptionalObservable<RadioButton<T>> _sourceObservable;

                public RadioGroupObservable(OptionalObservable<RadioButton<T>> sourceObservable)
                {
                    _sourceObservable = sourceObservable;
                }

                public T Data => _sourceObservable.Data.Model.Value.Data;
                public bool IsDefault => _sourceObservable.IsDefault || _sourceObservable.Data.Model.Value.IsDefault;

                public IDisposable Subscribe(Action<T> action, Action onNull)
                {
                    IDisposable last = null;
                    var disposeSubscribe = _sourceObservable.Subscribe(button =>
                    {
                        if (last is object) last.Dispose();
                        Debug.Assert(!button.Model.Value.IsDefault);
                        action(button.Model.Value.Data);
                        last = button.Model.Value.Subscribe(action, onNull);
                    }, () =>
                    {
                        if (last is object) last.Dispose();
                        last = null;
                        onNull();
                    });

                    return new Reflector(() =>
                    {
                        if (last is object) last.Dispose();
                        last = null;
                        disposeSubscribe.Dispose();
                    });
                }
            }
        }
    }
}