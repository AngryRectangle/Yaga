using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Yaga.Utils;
using Yaga.Utils.Exceptions;

namespace Tests.Collections
{
    public class ObservableListTest
    {
        [Test]
        public void Add_OneValue()
        {
            const int testValue = 5;
            var list = new ObservableList<int>();

            list.Add(testValue);

            Assert.AreEqual(testValue, list[0]);
            Assert.AreEqual(1, list.Count);
        }

        [Test]
        public void Add_SameValueTwice()
        {
            const int testValue = 5;
            var list = new ObservableList<int>();

            list.Add(testValue);
            list.Add(testValue);

            Assert.AreEqual(testValue, list[0]);
            Assert.AreEqual(testValue, list[1]);
            Assert.AreEqual(2, list.Count);
        }

        [Test]
        public void Add_DifferentValues()
        {
            const int firstTestValue = 5;
            const int secondTestValue = 6;
            var list = new ObservableList<int>();

            list.Add(firstTestValue);
            list.Add(secondTestValue);

            Assert.AreEqual(firstTestValue, list[0]);
            Assert.AreEqual(secondTestValue, list[1]);
            Assert.AreEqual(2, list.Count);
        }

        [Test]
        public void Enumerable_Empty()
        {
            var list = new ObservableList<int>();

            Assert.IsTrue(!list.Any());
        }

        [Test]
        [TestCase(new[] { 1, 1 })]
        [TestCase(new[] { 1, 2 })]
        [TestCase(new[] { 1, 2, 3, 4, 5, 6 })]
        public void Enumerable(int[] source)
        {
            var list = new ObservableList<int>();
            foreach (var value in source)
                list.Add(value);

            Assert.IsTrue(source.SequenceEqual(list));
        }

        [Test]
        public void Clear_Empty()
        {
            var list = new ObservableList<int>();

            list.Clear();

            Assert.AreEqual(0, list.Count);
            Assert.IsTrue(!list.Any());
        }

        [TestCase(new[] { 1, 1 })]
        [TestCase(new[] { 1, 2 })]
        [TestCase(new[] { 1, 2, 3, 4, 5, 6 })]
        public void Clear(int[] source)
        {
            var list = new ObservableList<int>();
            foreach (var value in source)
                list.Add(value);

            list.Clear();

            Assert.AreEqual(0, list.Count);
            Assert.IsTrue(!list.Any());
        }

        [Test]
        [TestCase(new[] { 1, 2 }, 0, 5, new[] { 5, 1, 2 })]
        [TestCase(new[] { 1, 2 }, 1, 5, new[] { 1, 5, 2 })]
        [TestCase(new[] { 1, 2 }, 2, 5, new[] { 1, 2, 5 })]
        public void Insert(int[] source, int insertIndex, int insertValue, int[] targetSequence)
        {
            var list = new ObservableList<int>();
            foreach (var value in source)
                list.Add(value);

            list.Insert(insertIndex, insertValue);

            Assert.IsTrue(targetSequence.SequenceEqual(list));
        }

        [Test]
        [TestCase(new[] { 1, 2 }, -1)]
        [TestCase(new[] { 1, 2 }, 3)]
        [TestCase(new int[] { }, 1)]
        public void Insert_ArgumentOutOfRange(int[] source, int insertIndex)
        {
            var list = new ObservableList<int>();
            foreach (var value in source)
                list.Add(value);

            Assert.Throws<ArgumentOutOfRangeException>(() => list.Insert(insertIndex, -1));
        }

        [Test]
        [TestCase(new[] { 1, 2 }, 0, new[] { 2 })]
        [TestCase(new[] { 1, 2 }, 1, new[] { 1 })]
        [TestCase(new[] { 1, 2, 3 }, 1, new[] { 1, 3 })]
        public void RemoveAt(int[] source, int removeIndex, int[] targetSequence)
        {
            var list = new ObservableList<int>();
            foreach (var value in source)
                list.Add(value);

            list.RemoveAt(removeIndex);

            Assert.IsTrue(targetSequence.SequenceEqual(list));
        }

        /// <summary>
        /// For some reason in Unity's list implementation,
        /// it throws <see cref="IndexOutOfRangeException"/> for <see cref="List.RemoveAt"/>,
        /// but <see cref="List.Insert"/> throws <see cref="ArgumentOutOfRangeException"/>
        /// </summary>
        /// <param name="source"></param>
        /// <param name="removeIndex"></param>
        [Test]
        [TestCase(new[] { 1, 2 }, -1)]
        [TestCase(new[] { 1, 2 }, 2)]
        [TestCase(new int[] { }, 0)]
        public void RemoveAt_IndexOutOfRange(int[] source, int removeIndex)
        {
            var list = new ObservableList<int>();
            foreach (var value in source)
                list.Add(value);

            Assert.Throws<IndexOutOfRangeException>(() => list.RemoveAt(removeIndex));
        }

        [Test]
        [TestCase(new[] { 1, 2 }, 1, new[] { 2 }, true)]
        [TestCase(new[] { 1, 2 }, 2, new[] { 1 }, true)]
        [TestCase(new[] { 1, 2 }, 3, new[] { 1, 2 }, false)]
        [TestCase(new[] { 1, 2, 3 }, 2, new[] { 1, 3 }, true)]
        [TestCase(new[] { 1, 2, 3, 2 }, 2, new[] { 1, 3, 2 }, true)]
        public void Remove(int[] source, int removeValue, int[] targetSequence, bool expectedResult)
        {
            var list = new ObservableList<int>();
            foreach (var value in source)
                list.Add(value);

            var actualResult = list.Remove(removeValue);

            Assert.AreEqual(expectedResult, actualResult);
            Assert.IsTrue(targetSequence.SequenceEqual(list));
        }

        [Test]
        [TestCase(new[] { 1, 2 }, 0, 3, new[] { 3, 2 })]
        [TestCase(new[] { 1, 2, 3 }, 1, 3, new[] { 1, 3, 3 })]
        [TestCase(new[] { 1, 2, 3 }, 2, 3, new[] { 1, 2, 3 })]
        public void Setter(int[] source, int index, int setValue, int[] targetSequence)
        {
            var list = new ObservableList<int>();
            foreach (var value in source)
                list.Add(value);

            list[index] = setValue;

            Assert.IsTrue(targetSequence.SequenceEqual(list));
        }

        [Test]
        public void GetObservable()
        {
            var list = new ObservableList<int>();
            list.Add(5);
            list.Add(6);
            list.Add(7);

            var observable = list.GetObservable(1);

            list[1] = 8;
            Assert.AreEqual(8, observable.Data);

            list.Insert(1, 6);
            Assert.AreEqual(8, observable.Data);
        }

        [Test]
        public void GetObservable_InvalidIndex()
        {
            var list = new ObservableList<int>();
            list.Add(5);
            list.Add(6);
            list.Add(7);

            Assert.Throws<IndexOutOfRangeException>(() => list.GetObservable(-1));
            Assert.Throws<IndexOutOfRangeException>(() => list.GetObservable(3));
        }

        [Test]
        public void GetObservable_GetData()
        {
            var list = new ObservableList<int>();
            list.Add(5);
            list.Add(6);
            list.Add(7);

            var observable = list.GetObservable(1);

            Assert.IsFalse(observable.IsDefault);
            Assert.AreEqual(6, observable.Data);
        }

        [Test]
        public void GetObservable_GetDataAfterInsertBefore()
        {
            var list = new ObservableList<int>();
            list.Add(5);
            list.Add(6);
            list.Add(7);

            var observable = list.GetObservable(1);
            list.Insert(0, 4);

            Assert.IsFalse(observable.IsDefault);
            Assert.AreEqual(6, observable.Data);
        }

        [Test]
        public void GetObservable_GetDataAfterInsertBeforeAndValueChange()
        {
            var list = new ObservableList<int>();
            list.Add(5);
            list.Add(6);
            list.Add(7);

            var observable = list.GetObservable(1);
            list.Insert(0, 4);
            list[2] = 8;

            Assert.IsFalse(observable.IsDefault);
            Assert.AreEqual(8, observable.Data);
        }

        [Test]
        public void GetObservable_GetDataAfterValueChange()
        {
            var list = new ObservableList<int>();
            list.Add(5);
            list.Add(6);
            list.Add(7);

            var observable = list.GetObservable(1);
            list[1] = 8;

            Assert.IsFalse(observable.IsDefault);
            Assert.AreEqual(8, observable.Data);
        }

        [Test]
        public void GetObservable_GetDataAfterRemove()
        {
            var list = new ObservableList<int>();
            list.Add(5);
            list.Add(6);
            list.Add(7);

            var observable = list.GetObservable(1);
            list.RemoveAt(1);

            Assert.IsTrue(observable.IsDefault);
            Assert.Throws<EmptyDataAccessException>(() =>
            {
                var m = observable.Data;
            });
        }

        [Test]
        public void Sort()
        {
            var list = new ObservableList<int>();
            new List<int>().Sort();
        }
    }
}