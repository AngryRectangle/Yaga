using System;

namespace Yaga.Exceptions
{
    public class InitializationException : Exception
    {
        public InitializationException(string message) : base(message)
        {
        }
    }
}