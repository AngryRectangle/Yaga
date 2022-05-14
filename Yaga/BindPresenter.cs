namespace Yaga
{
    public abstract class BindPresenter<TView, TModel> : Presenter<TView, TModel>
        where TView : View<TModel>
    {
        protected override void OnModelSet(TView view, TModel model)
        {
            foreach (var bind in view.Bindings) bind.Apply(model);
        }

        protected override void OnModelUnset(TView view)
        {
            foreach (var bind in view.Bindings) bind.DeApply();
        }
    }
}