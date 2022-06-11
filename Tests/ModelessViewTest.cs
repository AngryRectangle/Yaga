using System;
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Yaga.Test
{
    public class ModelessViewTest : BaseUiTest
    {
        [SetUp]
        public void SetUp()
        {
            UiBootstrap.InitializeSingleton();
            UiControl.InitializeSingleton(Locator.canvasPrefab);
        }
        
        [UnityTest]
        public IEnumerator ModelessViewSet()
        {
            UiBootstrap.Bind<PresenterSetTest>();
            UiControl.Instance.Create(Locator.modelessView);
            yield return null;
            Assert.Fail("OnSet method was not called");
        }

        public class PresenterSetTest : Presenter<ModelessView>
        {
            protected override void OnSet(ModelessView view)
            {
                Assert.Pass();
            }
        }

        [UnityTest]
        public IEnumerator ModelessViewUnset()
        {
            UiBootstrap.Bind(new PresenterUnsetTest(Assert.Pass));
            var instance = UiControl.Instance.Create(Locator.modelessView);
            yield return null;
            UiBootstrap.Instance.Unset(instance);
            Assert.Fail("OnUnset method was not called");
        }

        [UnityTest]
        public IEnumerator ModelessViewUnsetOnDestroy()
        {
            var unSetWasCalled = false;
            UiBootstrap.Bind(new PresenterUnsetTest(() => unSetWasCalled = true));
            var instance = UiControl.Instance.Create(Locator.modelessView);
            yield return null;
            GameObject.Destroy(instance);
            yield return null;
            Assert.True(unSetWasCalled, "OnUnset method was not called");
        }

        public class PresenterUnsetTest : Presenter<ModelessView>
        {
            private readonly Action _onUnset;

            public PresenterUnsetTest(Action onUnset)
            {
                _onUnset = onUnset;
            }

            protected override void OnUnset(ModelessView view)
            {
                _onUnset();
            }
        }
    }
}