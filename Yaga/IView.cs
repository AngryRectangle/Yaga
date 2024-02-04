using System;
using Optional;
using UnityEngine;

namespace Yaga
{
    public interface IView : IEquatable<IView>
    {
        int GetInstanceID();
        bool IsPrefab { get; }
        void Destroy();
        IView Create(RectTransform parent, bool isRoot = false);
        internal Option<Subscriptions> Model { get; }
    }

    public interface IView<TModel> : IView
    {
        internal new Option<(TModel Model, Subscriptions Subs)> Model { get; set; }
    }
}