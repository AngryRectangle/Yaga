using System;

namespace Yaga
{
    public class MultiplePresenterException : Exception
    {
        public MultiplePresenterException(Type viewType) : base(
            $"For view type {viewType} are more then one acceptable presenters")
        {
        }
    }
}