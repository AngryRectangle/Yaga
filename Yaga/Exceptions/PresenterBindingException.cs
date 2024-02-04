using System;

namespace Yaga.Exceptions
{
    public class PresenterBindingException : Exception
    {
        public readonly Type PresenterType;

        public PresenterBindingException(string message, Type presenterType) : base(message)
        {
            PresenterType = presenterType;
        }
    }
}