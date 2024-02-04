using NUnit.Framework;

namespace Tests.Observables
{
    public class BeaconTest
    {
        [Test]
        public void Constructor_Default()
        {
            var beacon = new Yaga.Reactive.Beacon();
            beacon.Execute();
        }
        
        [Test]
        public void Constructor_Action()
        {
            var result = 0;
            var beacon = new Yaga.Reactive.Beacon(() => result = 42);
            beacon.Execute();
            Assert.AreEqual(42, result);
        }
        
        [Test]
        public void Add()
        {
            var result = 0;
            var beacon = new Yaga.Reactive.Beacon();
            beacon.Add(() => result = 42);
            beacon.Execute();
            Assert.AreEqual(42, result);
        }
        
        [Test]
        public void Remove()
        {
            var result = 0;
            var beacon = new Yaga.Reactive.Beacon();
            var action = new System.Action(() => result = 42);
            beacon.Add(action);
            beacon.Remove(action);
            beacon.Execute();
            Assert.AreEqual(0, result);
        }
        
        [Test]
        public void Add_Unsubscribe()
        {
            var result = 0;
            var beacon = new Yaga.Reactive.Beacon();
            var disposable = beacon.Add(() => result = 42);
            disposable.Dispose();
            beacon.Execute();
            Assert.AreEqual(0, result);
        }
        
        [Test]
        public void Operator()
        {
            var result = 0;
            var beacon = new Yaga.Reactive.Beacon();
            var unsubscription = beacon + (() => result = 42);
            beacon.Execute();
            Assert.AreEqual(42, result);
        }
        
        [Test]
        public void Operator_Unsubscribe()
        {
            var result = 0;
            var beacon = new Yaga.Reactive.Beacon();
            var unsubscription = beacon + (() => result = 42);
            unsubscription.Dispose();
            beacon.Execute();
            Assert.AreEqual(0, result);
        }
        
        [Test]
        public void Constructor_Default_T1()
        {
            var beacon = new Yaga.Reactive.Beacon<int>();
            beacon.Execute(42);
        }
        
        [Test]
        public void Constructor_Action_T1()
        {
            var result = 0;
            var beacon = new Yaga.Reactive.Beacon<int>(value => result = value);
            beacon.Execute(42);
            Assert.AreEqual(42, result);
        }
        
        [Test]
        public void Add_T1()
        {
            var result = 0;
            var beacon = new Yaga.Reactive.Beacon<int>();
            beacon.Add(value => result = value);
            beacon.Execute(42);
            Assert.AreEqual(42, result);
        }
        
        [Test]
        public void Remove_T1()
        {
            var result = 0;
            var beacon = new Yaga.Reactive.Beacon<int>();
            var action = new System.Action<int>(value => result = value);
            beacon.Add(action);
            beacon.Remove(action);
            beacon.Execute(42);
            Assert.AreEqual(0, result);
        }
        
        [Test]
        public void Add_Unsubscribe_T1()
        {
            var result = 0;
            var beacon = new Yaga.Reactive.Beacon<int>();
            var disposable = beacon.Add(value => result = value);
            disposable.Dispose();
            beacon.Execute(42);
            Assert.AreEqual(0, result);
        }
        
        [Test]
        public void Operator_T1()
        {
            var result = 0;
            var beacon = new Yaga.Reactive.Beacon<int>();
            var unsubscription = beacon + (value => result = value);
            beacon.Execute(42);
            Assert.AreEqual(42, result);
        }
        
        [Test]
        public void Operator_Unsubscribe_T1()
        {
            var result = 0;
            var beacon = new Yaga.Reactive.Beacon<int>();
            var unsubscription = beacon + (value => result = value);
            unsubscription.Dispose();
            beacon.Execute(42);
            Assert.AreEqual(0, result);
        }
    }
}