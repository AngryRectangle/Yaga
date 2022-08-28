using System;

namespace Yaga.Exceptions
{
    public class PresenterBindingException : Exception
    {
        public readonly Type PresenterType;

        public PresenterBindingException(Type presenterType)
        {
            PresenterType = presenterType;
        }

        public PresenterBindingException(string message, Type presenterType) : base(message)
        {
            PresenterType = presenterType;
        }

        public PresenterBindingException(string message, Exception innerException, Type presenterType) : base(message,
            innerException)
        {
            PresenterType = presenterType;
        }
    }
}