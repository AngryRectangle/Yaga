using System;
using Optional;

namespace Yaga
{
    public interface IPresenter
    {
        internal Subscriptions Set(IView view, object model);
    }

    public interface IPresenter<in TView, in TModel> : IPresenter
        where TView : IView<TModel>
    {
        internal Subscriptions Set(TView view, TModel model);
    }

    public abstract class Presenter<TView, TModel> : IPresenter<TView, TModel>
        where TView : IView<TModel>
    {
        Subscriptions IPresenter.Set(IView view, object model)
        {
            return ((IPresenter<TView, TModel>)this).Set((TView)view, (TModel)model);
        }

        Subscriptions IPresenter<TView, TModel>.Set(TView view, TModel model)
        {
            var subscriptions = new Subscriptions();
            view.Model.MatchSome(active => active.Subs.Dispose());
            view.Model = (model, subscriptions).Some();
            OnSet(view, model, subscriptions);
            subscriptions.Add(new Disposable(() =>
            {
                view.Model = Option.None<(TModel, Subscriptions)>();
                OnUnset(view);
            }));
            return subscriptions;
        }

        /// <summary>
        /// Invoked when model for view is being set.
        /// </summary>
        /// <param name="view">View instance</param>
        /// <param name="model">Model instance</param>
        /// <param name="subs">Object that stores subscription which will be canceled during model unset</param>
        protected abstract void OnSet(TView view, TModel model, ISubscriptions subs);
        /// <summary>
        /// Invoked when model for view is being unset.
        /// </summary>
        /// <param name="view">View instance</param>

        protected virtual void OnUnset(TView view)
        {
        }
    }

    public abstract class Presenter<TView> : IPresenter<TView, Unit>
        where TView : IView<Unit>
    {
        Subscriptions IPresenter.Set(IView view, object model)
        {
            return ((IPresenter<TView, Unit>)this).Set((TView)view, (Unit)model);
        }

        Subscriptions IPresenter<TView, Unit>.Set(TView view, Unit model)
        {
            var subscriptions = new Subscriptions();
            view.Model.MatchSome(active => active.Subs.Dispose());
            view.Model = (model, subscriptions).Some();
            OnSet(view, subscriptions);
            subscriptions.Add(new Disposable(() =>
            {
                view.Model = Option.None<(Unit, Subscriptions)>();
                OnUnset(view);
            }));
            return subscriptions;
        }

        /// <summary>
        /// Invoked when model for view is being set.
        /// </summary>
        /// <param name="view">View instance</param>
        /// <param name="subs">Object that stores subscription which will be canceled during model unset</param>
        protected virtual void OnSet(TView view, ISubscriptions subs)
        {
        }

        /// <summary>
        /// Invoked when model for view is being unset.
        /// </summary>
        /// <param name="view">View instance</param>
        protected virtual void OnUnset(TView view)
        {
        }
    }
}