using System;
using System.Collections.Generic;

namespace Yaga
{
    public interface IView : IEquatable<IView>, IEnumerable<IView>
    {
        IEnumerable<IView> Children { get; }
        int GetInstanceID();
        bool IsOpened { get; }
        bool IsInstanced { get; }
        void Open();
        void Close();
        void Create();
        void Destroy();
    }

    public interface IView<TModel> : IView
    {
        bool HasModel { get; set; }
        TModel Model { get; set; }
    }
}