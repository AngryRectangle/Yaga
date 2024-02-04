using System.Collections;
using NUnit.Framework;
using Tests.Presenters;
using UnityEngine.TestTools;
using Yaga;
using Yaga.Test;
using Yaga.Test.Documentation;

namespace Tests
{
    public class SubscriptionsExtensionTest : BaseUiTest
    {
        [SetUp]
        public void SetUp()
        {
            UiBootstrap.InitializeSingleton();
            UiControl.InitializeSingleton(Locator.canvasPrefab);
        }

        [Test]
        public void Create_ChildCreated()
        {
            var childWasSet = false;
            
            var presenter = new PresenterWithChild(Locator, "Hello");
            var observablePresenter = new ObservablePresenter<SimpleTextButtonView, string>(_ => childWasSet = true);
            UiBootstrap.Instance.Bind(presenter);
            UiBootstrap.Instance.Bind(observablePresenter);

            var view = UiControl.Instance.Create(Locator.modelessView);
            var child = view.View.transform.GetComponentInChildren<SimpleTextButtonView>();
            Assert.IsTrue(child != null);
            Assert.IsTrue(childWasSet);
        }
        
        [Test]
        public void Create_ChildCreated_ModelUsed()
        {
            var presenter = new PresenterWithChild(Locator, "Hello");
            UiBootstrap.Instance.Bind(presenter);
            UiBootstrap.Instance.Bind<SimpleTextButtonView.Presenter>();

            var view = UiControl.Instance.Create(Locator.modelessView);
            var child = view.View.transform.GetComponentInChildren<SimpleTextButtonView>();
            Assert.AreEqual(child.Text.text, "Hello");
        }
        
        [Test]
        public void Create_UnitChildCreated()
        {
            var childWasSet = false;
            
            var presenter = new PresenterWithUnitChild(Locator);
            var observablePresenter = new ObservablePresenter<ModelessView>(_ => childWasSet = true);
            UiBootstrap.Instance.Bind(presenter);
            UiBootstrap.Instance.Bind(observablePresenter);

            var view = UiControl.Instance.Create(Locator.simpleTextButtonView, "Hello");
            var child = view.View.transform.GetComponentInChildren<ModelessView>();
            Assert.IsTrue(child != null);
            Assert.IsTrue(childWasSet);
        }
        
        [Test]
        public void Create_ChildCreatedAndUnset()
        {
            var childWasUnset = false;
            
            var presenter = new PresenterWithChild(Locator, "Hello");
            var observablePresenter = new ObservablePresenter<SimpleTextButtonView, string>(_ => {}, _ => childWasUnset = true);
            UiBootstrap.Instance.Bind(presenter);
            UiBootstrap.Instance.Bind(observablePresenter);

            var view = UiControl.Instance.Create(Locator.modelessView);
            var child = view.View.transform.GetComponentInChildren<SimpleTextButtonView>();
            Assert.IsTrue(child != null);
            Assert.IsFalse(childWasUnset);
            view.Unset();
            Assert.IsTrue(childWasUnset);
        }
        
        [Test]
        public void Create_UnitChildCreatedAndUnset()
        {
            var childWasUnset = false;
            
            var presenter = new PresenterWithUnitChild(Locator);
            var observablePresenter = new ObservablePresenter<ModelessView>(_ => {}, _ => childWasUnset = true);
            UiBootstrap.Instance.Bind(presenter);
            UiBootstrap.Instance.Bind(observablePresenter);

            var view = UiControl.Instance.Create(Locator.simpleTextButtonView, "Hello");
            var child = view.View.transform.GetComponentInChildren<ModelessView>();
            Assert.IsTrue(child != null);
            Assert.IsFalse(childWasUnset);
            view.Unset();
            Assert.IsTrue(childWasUnset);
        }

        [UnityTest]
        public IEnumerator Create_ChildDestroy_UnsubscriptionNotBeingInvokedAfter()
        {
            var childWasUnset = false;
            
            var presenter = new PresenterWithChild(Locator, "Hello");
            var observablePresenter = new ObservablePresenter<SimpleTextButtonView, string>(_ => {}, _ => childWasUnset = true);
            UiBootstrap.Instance.Bind(presenter);
            UiBootstrap.Instance.Bind(observablePresenter);

            var view = UiControl.Instance.Create(Locator.modelessView);
            var child = view.View.transform.GetComponentInChildren<SimpleTextButtonView>();
            child.Destroy();
            yield return null;
            Assert.IsTrue(childWasUnset);
            childWasUnset = false;
            
            view.Unset();
            Assert.IsFalse(childWasUnset);
        }
        
        [UnityTest]
        public IEnumerator Create_UnitChildDestroy_UnsubscriptionNotBeingInvokedAfter()
        {
            var childWasUnset = false;
            
            var presenter = new PresenterWithUnitChild(Locator);
            var observablePresenter = new ObservablePresenter<ModelessView>(_ => {}, _ => childWasUnset = true);
            UiBootstrap.Instance.Bind(presenter);
            UiBootstrap.Instance.Bind(observablePresenter);

            var view = UiControl.Instance.Create(Locator.simpleTextButtonView, "Hello");
            var child = view.View.transform.GetComponentInChildren<ModelessView>();
            child.Destroy();
            yield return null;
            Assert.IsTrue(childWasUnset);
            childWasUnset = false;
            
            view.Unset();
            Assert.IsFalse(childWasUnset);
        }

        private class PresenterWithChild : Presenter<ModelessView>
        {
            private readonly PrefabLocator _locator;
            private readonly string _defaultButtonText;

            public PresenterWithChild(PrefabLocator locator, string defaultButtonText)
            {
                _locator = locator;
                _defaultButtonText = defaultButtonText;
            }

            protected override void OnSet(ModelessView view, ISubscriptions subs)
            {
                subs.Create(_locator.simpleTextButtonView, _defaultButtonText, view);
            }
        }
        
        private class PresenterWithUnitChild : Presenter<SimpleTextButtonView, string>
        {
            private readonly PrefabLocator _locator;

            public PresenterWithUnitChild(PrefabLocator locator)
            {
                _locator = locator;
            }

            protected override void OnSet(SimpleTextButtonView view, string model, ISubscriptions subs)
            {
                subs.Create(_locator.modelessView, view);
            }
        }
    }
}