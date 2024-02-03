using System.Collections;
using NUnit.Framework;
using Tests.Presenters;
using UnityEngine;
using UnityEngine.TestTools;
using Yaga;
using Yaga.Test;
using Yaga.Test.Documentation;
using Object = UnityEngine.Object;

namespace Tests
{
    public class ViewTest : BaseUiTest
    {
        [SetUp]
        public void SetUp()
        {
            UiBootstrap.InitializeSingleton();
            UiControl.InitializeSingleton(Locator.canvasPrefab);
        }

        [Test]
        public void IsPrefab_WithModel_True()
        {
            Assert.IsTrue(((IView)Locator.simpleTextButtonView).IsPrefab);
        }

        [Test]
        public void IsPrefab_UnitModel_True()
        {
            Assert.IsTrue(((IView)Locator.modelessView).IsPrefab);
        }

        [Test]
        public void IsPrefab_WithModel_False()
        {
            Assert.IsFalse(((IView)Object.Instantiate(Locator.simpleTextButtonView)).IsPrefab);
        }

        [Test]
        public void IsPrefab_UnitModel_False()
        {
            Assert.IsFalse(((IView)Object.Instantiate(Locator.modelessView)).IsPrefab);
        }

        [Test]
        public void ViewWithModel_ShouldNotHaveModelInitially()
        {
            UiBootstrap.Instance.Bind<SimpleTextButtonView.Presenter>();
            var view = Object.Instantiate(Locator.simpleTextButtonView);
            Assert.IsFalse(((IView)view).Model.HasValue);
        }

        [Test]
        public void ViewUnitModel_ShouldNotHaveModelInitially()
        {
            UiBootstrap.Instance.Bind<SimpleTextButtonView.Presenter>();
            var view = Object.Instantiate(Locator.modelessView);
            Assert.IsFalse(((IView)view).Model.HasValue);
        }

        [Test]
        public void UnsetModel_WithModel_ShouldNotHaveModelAfterwards()
        {
            const string testModel = "testModel";
            UiBootstrap.Instance.Bind<SimpleTextButtonView.Presenter>();
            var viewControl = UiControl.Instance.Create(Locator.simpleTextButtonView, testModel);
            viewControl.Unset();
            Assert.IsFalse(((IView)viewControl.View).Model.HasValue);
        }

        [Test]
        public void UnsetModel_UnitModel_ShouldNotHaveModelAfterwards()
        {
            UiBootstrap.Instance.Bind(new ObservablePresenter<ModelessView>());
            var viewControl = UiControl.Instance.Create(Locator.modelessView);
            viewControl.Unset();
            Assert.IsFalse(((IView)viewControl.View).Model.HasValue);
        }

        [Test]
        public void SetModel_WithModel_ShouldInvokeModelSet()
        {
            var invokedSet = false;
            const string testModel = "testModel";
            UiBootstrap.Instance.Bind(new ObservablePresenter<SimpleTextButtonView, string>(_ => invokedSet = true));
            var viewControl = UiControl.Instance.Create(Locator.simpleTextButtonView, testModel);
            Assert.IsTrue(invokedSet);
            invokedSet = false;
            viewControl.Set(testModel);
            Assert.IsTrue(invokedSet);
        }

        [Test]
        public void SetModel_UnitModel_ShouldInvokeModelSet()
        {
            var invokedSet = false;
            UiBootstrap.Instance.Bind(new ObservablePresenter<ModelessView>(_ => invokedSet = true));
            var viewControl = UiControl.Instance.Create(Locator.modelessView);
            Assert.IsTrue(invokedSet);
            invokedSet = false;
            viewControl.Set(Unit.Instance);
            Assert.IsTrue(invokedSet);
        }

        [UnityTest]
        public IEnumerator Destroy_WithModel_ShouldDestroyGameObject()
        {
            UiBootstrap.Instance.Bind(new ObservablePresenter<SimpleTextButtonView, string>());
            var viewControl = UiControl.Instance.Create(Locator.simpleTextButtonView, "testModel");
            viewControl.View.Destroy();
            yield return null;
            // I can't ise Assert.IsNull because GameObject actually is not null, because "==" operator is overrode.
            Assert.IsTrue(viewControl.View == null);
        }

        [UnityTest]
        public IEnumerator Destroy_UnitModel_ShouldDestroyGameObject()
        {
            UiBootstrap.Instance.Bind(new ObservablePresenter<ModelessView>());
            var viewControl = UiControl.Instance.Create(Locator.modelessView);
            viewControl.View.Destroy();
            yield return null;
            // I can't ise Assert.IsNull because GameObject actually is not null, because "==" operator is overrode.
            Assert.IsTrue(viewControl.View == null);
        }

        [UnityTest]
        public IEnumerator Destroy_WithModel_ShouldDisposeModel()
        {
            var invokedDispose = false;
            UiBootstrap.Instance.Bind(
                new ObservablePresenter<SimpleTextButtonView, string>(_ => { }, _ => invokedDispose = true));
            var viewControl = UiControl.Instance.Create(Locator.simpleTextButtonView, "testModel");
            viewControl.View.Destroy();
            yield return null;
            Assert.IsTrue(invokedDispose);
        }

        [UnityTest]
        public IEnumerator Destroy_UnitModel_ShouldDisposeModel()
        {
            var invokedDispose = false;
            UiBootstrap.Instance.Bind(new ObservablePresenter<ModelessView>(_ => { }, _ => invokedDispose = true));
            var viewControl = UiControl.Instance.Create(Locator.modelessView);
            viewControl.View.Destroy();
            yield return null;
            Assert.IsTrue(invokedDispose);
        }
        
        [UnityTest]
        public IEnumerator Destroy_WithModel_ShouldDestroyRootParent()
        {
            UiBootstrap.Instance.Bind(new ObservablePresenter<SimpleTextButtonView, string>());
            var parent = Object.Instantiate(Locator.canvasPrefab);
            var viewControl = UiControl.Instance.Create(Locator.simpleTextButtonView, "testModel", (RectTransform)parent.transform, true);
            viewControl.View.Destroy();
            yield return null;
            // I can't ise Assert.IsNull because GameObject actually is not null, because "==" operator is overrode.
            Assert.IsTrue(parent == null);
        }
        
        [UnityTest]
        public IEnumerator Destroy_UnitModel_ShouldDestroyRootParent()
        {
            UiBootstrap.Instance.Bind(new ObservablePresenter<ModelessView>());
            var parent = Object.Instantiate(Locator.canvasPrefab);
            var viewControl = UiControl.Instance.Create(Locator.modelessView, (RectTransform)parent.transform, true);
            viewControl.View.Destroy();
            yield return null;
            // I can't ise Assert.IsNull because GameObject actually is not null, because "==" operator is overrode.
            Assert.IsTrue(parent == null);
        }

        [UnityTest]
        public IEnumerator Destroy_WithModel_ShouldNotDestroyNotRootParent()
        {
            UiBootstrap.Instance.Bind(new ObservablePresenter<SimpleTextButtonView, string>());
            var parent = Object.Instantiate(Locator.canvasPrefab);
            var viewControl = UiControl.Instance.Create(Locator.simpleTextButtonView, "testModel", (RectTransform)parent.transform);
            viewControl.View.Destroy();
            yield return null;
            // I can't ise Assert.IsNull because GameObject actually is not null, because "==" operator is overrode.
            Assert.IsTrue(parent != null);
        }
        
        [UnityTest]
        public IEnumerator Destroy_UnitModel_ShouldNotDestroyNotRootParent()
        {
            UiBootstrap.Instance.Bind(new ObservablePresenter<ModelessView>());
            var parent = Object.Instantiate(Locator.canvasPrefab);
            var viewControl = UiControl.Instance.Create(Locator.modelessView, (RectTransform)parent.transform);
            viewControl.View.Destroy();
            yield return null;
            // I can't ise Assert.IsNull because GameObject actually is not null, because "==" operator is overrode.
            Assert.IsTrue(parent != null);
        }
        
        [Test]
        public void Equals_WithModel_ShouldBeEqual()
        {
            UiBootstrap.Instance.Bind(new ObservablePresenter<SimpleTextButtonView, string>());
            var viewControl = UiControl.Instance.Create(Locator.simpleTextButtonView, "testModel");
            Assert.IsTrue(viewControl.View.Equals(viewControl.View));
        }
        
        [Test]
        public void Equals_UnitModel_ShouldBeEqual()
        {
            UiBootstrap.Instance.Bind(new ObservablePresenter<ModelessView>());
            var viewControl = UiControl.Instance.Create(Locator.modelessView);
            Assert.IsTrue(viewControl.View.Equals(viewControl.View));
        }
        
        [Test]
        public void Equals_WithModel_ShouldNotBeEqual()
        {
            UiBootstrap.Instance.Bind(new ObservablePresenter<SimpleTextButtonView, string>());
            var viewControl = UiControl.Instance.Create(Locator.simpleTextButtonView, "testModel");
            var viewControl2 = UiControl.Instance.Create(Locator.simpleTextButtonView, "testModel");
            Assert.IsFalse(viewControl.View.Equals(viewControl2.View));
        }
        
        [Test]
        public void Equals_UnitModel_ShouldNotBeEqual()
        {
            UiBootstrap.Instance.Bind(new ObservablePresenter<ModelessView>());
            var viewControl = UiControl.Instance.Create(Locator.modelessView);
            var viewControl2 = UiControl.Instance.Create(Locator.modelessView);
            Assert.IsFalse(viewControl.View.Equals(viewControl2.View));
        }
    }
}