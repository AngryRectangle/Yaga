using System;
using System.Collections;
using NUnit.Framework;
using Tests.Presenters;
using UnityEngine;
using UnityEngine.TestTools;
using Yaga;
using Yaga.Test;

namespace Tests
{
    public class UiControlTest : BaseUiTest
    {
        [SetUp]
        public void SetUp()
        {
            UiBootstrap.InitializeSingleton();
            UiBootstrap.Bind<TestPresenter>();
            UiControl.InitializeSingleton(Locator.canvasPrefab);
        }

        [Test]
        public void CheckCanvasCreation()
        {
            var view = UiControl.Instance.Create(Locator.modelessView);
            Assert.IsTrue(view.transform.parent is object);

            var viewParent = view.transform.parent.gameObject;
            Assert.IsTrue(viewParent.TryGetComponent<Canvas>(out _));
        }

        [UnityTest]
        public IEnumerator CheckViewDestroying()
        {
            var view = UiControl.Instance.Create(Locator.modelessView);
            UiControl.Instance.Destroy(view);
            yield return null;
            // I can't ise Assert.IsNull because GameObject actually is not null, because "==" operator is overrode.
            Assert.IsTrue(view == null);
        }
        
        [UnityTest]
        public IEnumerator CheckCanvasDestroying()
        {
            var view = UiControl.Instance.Create(Locator.modelessView);
            var viewParent = view.transform.parent.gameObject;
            var canvas = viewParent.GetComponent<Canvas>();
            UiControl.Instance.Destroy(view);
            yield return null;
            // I can't ise Assert.IsNull because GameObject actually is not null, because "==" operator is overrode.
            Assert.IsTrue(canvas == null);
            Assert.IsTrue(viewParent == null);
        }

        private class TestPresenter : ObservableModelessPresenter<ModelessView>
        {
        }
    }
}