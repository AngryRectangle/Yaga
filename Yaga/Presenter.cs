﻿using System;
using Optional;

namespace Yaga
{
    public interface IPresenter
    {
        bool AcceptableView(Type viewType);
    }

    // This interfaces is needed to avoid c# generics stupidity on UiBootstrap.Set method.
    public interface IPresenterWithUnspecifiedView : IPresenter
    {
        Subscriptions Set(IView view, object model);
    }

    public interface IPresenter<in TView, in TModel> : IPresenterWithUnspecifiedView
        where TView : IView<TModel>
    {
        Subscriptions Set(TView view, TModel model);
    }

    public abstract class Presenter<TView, TModel> : IPresenter<TView, TModel>
        where TView : IView<TModel>
    {
        public Subscriptions Set(IView view, object model)
        {
            return Set((TView)view, (TModel)model);
        }

        public Subscriptions Set(TView view, TModel model)
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

        protected abstract void OnSet(TView view, TModel model, ISubscriptionsOwner subs);

        protected virtual void OnUnset(TView view)
        {
        }

        public bool AcceptableView(Type viewType)
            => viewType == typeof(TView) || typeof(TView).IsAssignableFrom(viewType);
    }

    public abstract class Presenter<TView> : IPresenter<TView, Unit>
        where TView : IView<Unit>
    {
        public Subscriptions Set(IView view, object model)
        {
            return Set((TView)view, (Unit)model);
        }

        public Subscriptions Set(TView view, Unit model)
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

        protected virtual void OnSet(TView view, ISubscriptionsOwner subs)
        {
        }

        protected virtual void OnUnset(TView view)
        {
        }

        public bool AcceptableView(Type viewType)
            => viewType == typeof(TView) || typeof(TView).IsAssignableFrom(viewType);
    }
}