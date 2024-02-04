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
            Assert.AreEqual("Hello", child.Text.text);
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
            var observablePresenter =
                new ObservablePresenter<SimpleTextButtonView, string>(_ => { }, _ => childWasUnset = true);
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
            var observablePresenter = new ObservablePresenter<ModelessView>(_ => { }, _ => childWasUnset = true);
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
            var observablePresenter =
                new ObservablePresenter<SimpleTextButtonView, string>(_ => { }, _ => childWasUnset = true);
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
            var observablePresenter = new ObservablePresenter<ModelessView>(_ => { }, _ => childWasUnset = true);
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

        [Test]
        public void Set_ModelSetForChildren()
        {
            var setOnViewWithModel = false;
            var setOnViewWithoutModel = false;
            var presenterWithModel =
                new ObservablePresenter<SimpleTextButtonView, string>(_ => setOnViewWithModel = true);
            var presenterWithoutModel =
                new ObservablePresenter<ModelessView>(_ => setOnViewWithoutModel = true);
            var presenterWithModelChildren = new ChildrenViewPresenter("Hello");

            UiBootstrap.Instance.Bind(presenterWithModel);
            UiBootstrap.Instance.Bind(presenterWithoutModel);
            UiBootstrap.Instance.Bind(presenterWithModelChildren);

            UiControl.Instance.Create(Locator.viewWithChild);

            Assert.True(setOnViewWithModel);
            Assert.True(setOnViewWithoutModel);
        }

        [Test]
        public void Set_ModelSetForChildren_ModelUsed()
        {
            var presenterWithoutModel = new ObservablePresenter<ModelessView>();
            var presenterWithModelChildren = new ChildrenViewPresenter("Hello");

            UiBootstrap.Instance.Bind<SimpleTextButtonView.Presenter>();
            UiBootstrap.Instance.Bind(presenterWithoutModel);
            UiBootstrap.Instance.Bind(presenterWithModelChildren);

            var viewControl = UiControl.Instance.Create(Locator.viewWithChild);

            Assert.AreEqual("Hello", viewControl.View.viewWithModel.Text.text);
        }

        [Test]
        public void Set_ModelUnsetForChildren()
        {
            var unsetOnViewWithModel = false;
            var unsetOnViewWithoutModel = false;
            var presenterWithModel =
                new ObservablePresenter<SimpleTextButtonView, string>(_ => { }, _ => unsetOnViewWithModel = true);
            var presenterWithoutModel =
                new ObservablePresenter<ModelessView>(_ => { }, _ => unsetOnViewWithoutModel = true);
            var presenterWithModelChildren = new ChildrenViewPresenter("Hello");

            UiBootstrap.Instance.Bind(presenterWithModel);
            UiBootstrap.Instance.Bind(presenterWithoutModel);
            UiBootstrap.Instance.Bind(presenterWithModelChildren);

            var viewControl = UiControl.Instance.Create(Locator.viewWithChild);
            viewControl.Unset();

            Assert.True(unsetOnViewWithModel);
            Assert.True(unsetOnViewWithoutModel);
        }

        [Test]
        public void Set_ModelUnsetForChildren_UnsubscriptionNotBeingInvokedAfter()
        {
            var unsetOnViewWithModel = false;
            var unsetOnViewWithoutModel = false;
            var presenterWithModel =
                new ObservablePresenter<SimpleTextButtonView, string>(_ => { }, _ => unsetOnViewWithModel = true);
            var presenterWithoutModel =
                new ObservablePresenter<ModelessView>(_ => { }, _ => unsetOnViewWithoutModel = true);
            var presenterWithModelChildren = new ChildrenViewPresenter("Hello");

            UiBootstrap.Instance.Bind(presenterWithModel);
            UiBootstrap.Instance.Bind(presenterWithoutModel);
            UiBootstrap.Instance.Bind(presenterWithModelChildren);

            var viewControl = UiControl.Instance.Create(Locator.viewWithChild);
            presenterWithModelChildren.modelessView.Unset();
            presenterWithModelChildren.viewWithModel.Unset();
            unsetOnViewWithModel = false;
            unsetOnViewWithoutModel = false;
            viewControl.Unset();

            Assert.IsFalse(unsetOnViewWithModel);
            Assert.IsFalse(unsetOnViewWithoutModel);
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

        private class ChildrenViewPresenter : Presenter<ViewWithChild>
        {
            public ViewControl<ModelessView, Unit> modelessView;
            public ViewControl<SimpleTextButtonView, string> viewWithModel;

            private readonly string _defaultModel;

            public ChildrenViewPresenter(string defaultModel)
            {
                _defaultModel = defaultModel;
            }

            protected override void OnSet(ViewWithChild view, ISubscriptions subs)
            {
                modelessView = subs.Set(view.modelessView);
                viewWithModel = subs.Set(view.viewWithModel, _defaultModel);
            }
        }
    }
}