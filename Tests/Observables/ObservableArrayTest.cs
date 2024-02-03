using System;
using System.Linq;
using NUnit.Framework;
using Yaga.Reactive;

namespace Tests.Observables
{
    public class ObservableArrayTest
    {
        [Test]
        public void AmountFromConstructorOutOfRange()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new ObservableArray<int>(-1));
        }

        [Test]
        public void AmountFromConstructor()
        {
            var array = new ObservableArray<int>(5);
            Assert.AreEqual(5, array.Length);
        }

        [Test]
        public void AmountFromConstructorWithValues()
        {
            var array = new ObservableArray<int>(new[] { 1, 2, 3, 4, 5 });
            Assert.AreEqual(5, array.Length);
        }

        [Test]
        public void SubscriptionInvoke()
        {
            var wasInvoked = false;
            var array = new ObservableArray<int>(5);
            array.ItemSet.Add((index, last, next) =>
            {
                Assert.AreEqual(1, index);
                Assert.AreEqual(0, last);
                Assert.AreEqual(3, next);
                wasInvoked = true;
            });

            array[1] = 3;
            Assert.True(wasInvoked);
        }

        [Test]
        public void UnsubscriptionInvoke()
        {
            var array = new ObservableArray<int>(5);
            array[1] = 3;
            var disposable = array.ItemSet.Add((index, last, next) => Assert.Fail());
            disposable.Dispose();
            array[2] = 3;
            Assert.Pass();
        }

        [Test]
        public void ValuesFromConstructorWithValues()
        {
            var array = new ObservableArray<int>(new[] { 1, 2, 3, 4, 5 });
            Assert.AreEqual(5, array[4]);
        }

        [Test]
        public void OutOfRange()
        {
            var array = new ObservableArray<int>(5);
            Assert.Throws<IndexOutOfRangeException>(() => array[-1] = 5);
            Assert.Throws<IndexOutOfRangeException>(() => array[10] = 5);
        }

        [Test]
        public void OutOfRangeSubscriptionNotInvoking()
        {
            var array = new ObservableArray<int>(5);
            array.ItemSet.Add((index, last, next) => Assert.Fail());
            Assert.Throws<IndexOutOfRangeException>(() => array[-1] = 5);
        }

        [Test]
        public void Enumerable()
        {
            var array = new ObservableArray<int>(new[] { 5, 4, 6 });
            Assert.True(array.SequenceEqual(new[] { 5, 4, 6 }));
        }
    }
}