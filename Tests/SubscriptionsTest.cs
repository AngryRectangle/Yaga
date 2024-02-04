using NUnit.Framework;
using Yaga;

namespace Tests
{
    public class SubscriptionsTest
    {
        [Test]
        public void Add_DisposableAdded()
        {
            var subs = new Subscriptions();
            var disposable = new Disposable();
            var key = subs.Add(disposable);
            
            Assert.IsTrue(subs.Remove(key));
        }
        
        [Test]
        public void Add_Null_ThrowsException()
        {
            var subs = new Subscriptions();
            Assert.Catch<System.ArgumentNullException>(() => subs.Add(null));
        }
        
        [Test]
        public void Add_DisposableExecuted()
        {
            var wasExecuted = false;
            var subs = new Subscriptions();
            var disposable = new Disposable(() => wasExecuted = true);
            subs.Add(disposable);
            subs.Dispose();
            
            Assert.IsTrue(wasExecuted);
        }
        
        [Test]
        public void Remove_DisposableRemoved()
        {
            var wasExecuted = false;
            var subs = new Subscriptions();
            var disposable = new Disposable(() => wasExecuted = true);
            var key = subs.Add(disposable);
            Assert.IsTrue(subs.Remove(key));
            subs.Dispose();
            Assert.IsFalse(wasExecuted);
        }
        
        [Test]
        public void Remove_InvalidKey_ReturnsFalse()
        {
            var subs = new Subscriptions();
            var disposable = new Disposable();
            var key = subs.Add(disposable);
            Assert.IsFalse(subs.Remove(new ISubscriptions.Key(42)));
        }
    }
}