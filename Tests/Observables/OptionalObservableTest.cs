using NUnit.Framework;
using Optional;
using Yaga.Reactive;

namespace Tests.Observables
{
    /// <summary>
    /// OptionalObservable is just a wrapper around <see cref="Observable$lt;Option&lt;T>>"/>,
    /// so we only need to test the additional functionality.
    /// </summary>
    public class OptionalObservableTest
    {
        [Test]
        public void Constructor_DefaultValue()
        {
            var observable = new OptionalObservable<int>();
            Assert.AreEqual(Option.None<int>(), observable.Value);
        }
        
        [Test]
        public void Constructor_InitialValue()
        {
            var observable = new OptionalObservable<int>(42);
            Assert.AreEqual(42.Some(), observable.Value);
        }
        
        [Test]
        public void HasValue()
        {
            var observable = new OptionalObservable<int>();
            Assert.IsFalse(observable.HasValue);
            observable.Value = 42.Some();
            Assert.IsTrue(observable.HasValue);
        }
    }
}