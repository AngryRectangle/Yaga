using System;

namespace Yaga.Exceptions
{
    public class ViewModelIsUnsetException : Exception
    {
        public ViewModelIsUnsetException() : base("View was unset from model.")
        {
        }
    }
}