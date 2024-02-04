using NUnit.Framework;
using Yaga;
using Yaga.Reactive;

namespace Tests.Observables
{
    public class ObservableTest
    {
        [Test]
        public void Constructor_DefaultValue()
        {
            var observable = new Observable<int>();
            Assert.AreEqual(0, observable.Value);
        }

        [Test]
        public void Constructor_InitialValue()
        {
            var observable = new Observable<int>(42);
            Assert.AreEqual(42, observable.Value);
        }

        [Test]
        public void Set_Value()
        {
            var observable = new Observable<int>();
            observable.Value = 42;
            Assert.AreEqual(42, observable.Value);
        }

        [Test]
        public void Subscribe_Action()
        {
            var observable = new Observable<int>();
            var result = 0;
            observable.Subscribe(value => result = value);
            observable.Value = 42;
            Assert.AreEqual(42, result);
        }

        [Test]
        public void Subscribe_Action_Dispose()
        {
            var observable = new Observable<int>();
            var result = 0;
            var disposable = observable.Subscribe(value => result = value);
            disposable.Dispose();
            observable.Value = 42;
            Assert.AreEqual(0, result);
        }

        [Test]
        public void Subscribe_Observer()
        {
            var observable = new Observable<int>();
            var result = 0;
            observable.Subscribe(new ActionObserver<int>(value => result = value, null, null));
            observable.Value = 42;
            Assert.AreEqual(42, result);
        }
        
        [Test]
        public void Subscribe_Observer_Dispose()
        {
            var observable = new Observable<int>();
            var result = 0;
            var disposable = observable.Subscribe(new ActionObserver<int>(value => result = value, null, null));
            disposable.Dispose();
            observable.Value = 42;
            Assert.AreEqual(0, result);
        }
    }
}