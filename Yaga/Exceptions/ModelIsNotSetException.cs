using System;

namespace Yaga.Exceptions
{
    public class ModelIsNotSetException : Exception
    {
        public ModelIsNotSetException() : base(
            "Model is not set, but you are trying to access it. You should firstly set model and only then access it.")
        {
        }
    }
}