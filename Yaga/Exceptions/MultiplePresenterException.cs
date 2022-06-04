using System;

namespace Yaga.Exceptions
{
    /// <summary>
    /// Thrown when there are more then one acceptable <see cref="IPresenter"/> for view.
    /// </summary>
    public class MultiplePresenterException : Exception
    {
        public MultiplePresenterException(Type viewType) : base(
            $"For view type {viewType} are more then one acceptable presenters")
        {
        }
    }
}