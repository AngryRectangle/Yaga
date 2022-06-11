using System;

namespace Yaga.Utils.Exceptions
{
    public class EmptyDataAccessException : Exception
    {
        public EmptyDataAccessException() : base(
            "Attempt to access data which was set to default value or was not initialized. It is only allowed to access data that was set to not default value.")
        {
        }
    }
}