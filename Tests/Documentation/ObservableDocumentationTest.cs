using NUnit.Framework;
using UnityEngine;
using Yaga.Utils;

namespace Yaga.Test.Documentation
{
    public class ObservableDocumentationTest
    {
        [Test]
        public void EventToBeaconComparison()
        {
            Beacon<string> TextBeacon = new Beacon<string>();
            var disposable = TextBeacon.Add(e => Debug.Log($"Text {e} with length {e.Length}"));
            disposable.Dispose();
            Assert.Pass();
        }

        [Test]
        public void ObservableExample()
        {
            Observable<string> observable = new Observable<string>();
            var disposable = observable.Subscribe(e => Assert.AreEqual("second", e));

            observable.Data = "second";
            disposable.Dispose();
            observable.Data = "third";
        }

        [Test]
        public void BindingChainsExample()
        {
            var amount = new Observable<int>(100);
            var itemInfo = Observable.Bind(amount, count => $"Current amount is {count}");
            var disposable = itemInfo.Add(info => Assert.AreEqual("Current amount is 5", info));
            amount.Data = 5;

            disposable.Dispose();
            amount.Data = 8;
        }

        [Test]
        public void BindingChainsCombineExample()
        {
            var amount = new Observable<int>(100);
            var itemName = new Observable<string>("cake");
            var itemInfo = Observable.Bind(amount, itemName, (count, name) => $"{name} {count}");
            var disposable = itemInfo.Add(info => Assert.AreEqual("apple 100", info));
            itemName.Data = "apple";
            disposable.Dispose();
            disposable = itemInfo.Add(info => Assert.AreEqual("apple 5", info));
            amount.Data = 5;
            disposable.Dispose();
        }

        [Test]
        public void OptionalObservableExample()
        {
            var nullCalled = false;
            var valueCalled = false;

            var someObservable = new OptionalObservable<int>(100);
            var disposable = someObservable
                .Subscribe(value =>
                {
                    Assert.AreEqual(5, value);
                    valueCalled = true;
                }, () => nullCalled = true);

            Assert.AreEqual(false, nullCalled);
            Assert.AreEqual(false, valueCalled);
            someObservable.SetDefault();
            Assert.AreEqual(true, nullCalled);
            Assert.AreEqual(false, valueCalled);
            someObservable.Data = 5;
            Assert.AreEqual(true, valueCalled);

            disposable.Dispose();
            someObservable.Data = 6;
        }

        [Test]
        public void OptionalObservableChainExample()
        {
            var nullCalled = false;
            var valueCalled = false;
            
            var amount = new OptionalObservable<int>(100);
            var itemName = new OptionalObservable<string>("cake");
            var itemInfo = OptionalObservable.Bind(amount, itemName, (count, name) => $"{name} {count}");

            var disposable = itemInfo.Subscribe(info =>
            {
                valueCalled = true;
                Assert.AreEqual("apple 100", info);
            }, () => nullCalled = true);
            
            Assert.AreEqual(false, nullCalled);
            Assert.AreEqual(false, valueCalled);
            itemName.Data = "apple";
            Assert.AreEqual(false, nullCalled);
            Assert.AreEqual(true, valueCalled);
            amount.SetDefault();
            Assert.AreEqual(true, nullCalled);

            disposable.Dispose();
            amount.Data = 6;
        }
    }
}