using System;
using UnityEngine;

namespace Yaga.Exceptions
{
    /// <summary>
    /// Thrown when regular game object is being used as prefab.
    /// </summary>
    public class IsNotPrefabException : Exception
    {
        public GameObject GameObject { get; }

        public IsNotPrefabException(GameObject gameObject) : base($"Game object {gameObject} is not prefab.")
        {
            GameObject = gameObject;
        }
    }
}