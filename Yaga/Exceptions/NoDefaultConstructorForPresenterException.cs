using System;

namespace Yaga.Exceptions
{
    /// <summary>
    /// Thrown when there is no default constractor for <see cref="IPresenter"/>.
    /// </summary>
    public class NoDefaultConstructorForPresenterException : PresenterBindingException
    {
        public NoDefaultConstructorForPresenterException(Type presenterType, string message) : base(
            $"Default constructor was not found in {presenterType}. {message}", presenterType)
        {
        }
    }
}