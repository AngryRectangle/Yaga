using System;
using UnityEngine;

namespace Yaga.Exceptions
{
    /// <summary>
    /// Thrown when regular game object is being used as prefab.
    /// </summary>
    public class IsNotPrefabException : Exception
    {
        public IView View { get; }

        public IsNotPrefabException(IView view) : base($"View {view} is not prefab.")
        {
            View = view;
        }
    }
}