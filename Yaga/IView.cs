﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace Yaga
{
    public interface IView : IEquatable<IView>, IEnumerable<IView>, IDisposable
    {
        internal bool IsSetted { get; set; }
        IEnumerable<IView> Children { get; }
        int GetInstanceID();
        bool IsPrefab { get; }
        bool IsOpened { get; }
        void Open();
        void Close();
        IView Create(Transform parent);
        void Destroy();
    }

    public interface IView<TModel> : IView
    {
        bool HasModel { get; set; }
        TModel Model { get; internal set; }
        void Set(TModel model);
        void Unset(TModel model);
    }
}