using System;

namespace Yaga.Exceptions
{
    /// <summary>
    /// Thrown when there is no acceptable <see cref="IPresenter"/> for view.
    /// </summary>
    public class PresenterNotFoundException : Exception
    {
        public PresenterNotFoundException(Type view): base($"Presenter for {view} was not found.")
        {
        }
    }
}