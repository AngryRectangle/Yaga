using Yaga.Utils;

namespace Yaga
{
    public class RadioButton<T> : View<RadioButton<T>.RadioButtonModel>
    {
        public Beacon OnClick;

        private void Select()
        {
        }

        private void Deselect()
        {
        }

        public abstract class RadioButtonPresenter : Presenter<RadioButton<T>, RadioButtonModel>
        {
            protected override void OnModelSet(RadioButton<T> view, RadioButtonModel model)
            {
                view.SubscribeAndCall(model.Selected, isSelected =>
                {
                    if (isSelected)
                        view.Select();
                    else
                        view.Deselect();
                });

                view.SubscribeAndCall(model.Value, value => OnModelSet(view, value), () => { });
            }

            protected abstract void OnModelSet(RadioButton<T> view, T model);
        }

        public readonly struct RadioButtonModel
        {
            public readonly Observable<bool> Selected;
            public readonly IOptionalObservable<T> Value;

            public RadioButtonModel(bool selected, IOptionalObservable<T> value)
            {
                Selected = new Observable<bool>(selected);
                Value = value;
            }
        }
    }
}