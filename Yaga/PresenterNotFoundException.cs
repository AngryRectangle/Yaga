using System;
using System.Runtime.Serialization;
using JetBrains.Annotations;

namespace Yaga
{
    public class PresenterNotFoundException : Exception
    {
        public PresenterNotFoundException(Type view): base($"Presenter for {view} was not found.")
        {
        }
    }
}