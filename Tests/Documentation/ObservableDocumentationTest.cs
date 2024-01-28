using NUnit.Framework;
using Optional;
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
            var itemInfo = amount.Select(count => $"Current amount is {count}");
            var disposable = itemInfo.Subscribe(info => Assert.AreEqual("Current amount is 5", info));
            amount.Data = 5;

            disposable.Dispose();
            amount.Data = 8;
        }

        [Test]
        public void BindingChainsCombineExample()
        {
            var amount = new Observable<int>(100);
            var itemName = new Observable<string>("cake");
            var itemInfo = amount.CombineLatest(itemName, (count, name) => $"{name} {count}");
            var disposable = itemInfo.Subscribe(info => Assert.AreEqual("apple 100", info));
            itemName.Data = "apple";
            disposable.Dispose();
            disposable = itemInfo.Subscribe(info => Assert.AreEqual("apple 5", info));
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
            someObservable.Data = Option.None<int>();
            Assert.AreEqual(true, nullCalled);
            Assert.AreEqual(false, valueCalled);
            someObservable.Data = Option.Some(5);
            Assert.AreEqual(true, valueCalled);

            disposable.Dispose();
            someObservable.Data = Option.Some(6);
        }
    }
}