using System;

namespace Yaga.Exceptions
{
    /// <summary>
    /// Thrown when there are more then one acceptable <see cref="IPresenter"/> for view.
    /// </summary>
    public class MultiplePresenterException : Exception
    {
        public Type ViewType { get; }
        public Type ExistingPresenterType { get; }
        public Type ConflictingPresenterType { get; }

        public MultiplePresenterException(Type viewType, Type existingPresenterType, Type conflictingPresenterType) :
            base(
                $"Multiple presenters for view {viewType} found. Existing presenter: {existingPresenterType}, conflicting presenter: {conflictingPresenterType}")
        {
            ViewType = viewType;
            ExistingPresenterType = existingPresenterType;
            ConflictingPresenterType = conflictingPresenterType;
        }
    }
}