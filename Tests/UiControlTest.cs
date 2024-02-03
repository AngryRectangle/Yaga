using System;
using NUnit.Framework;
using Optional.Unsafe;
using Tests.Presenters;
using UnityEngine;
using Yaga;
using Yaga.Exceptions;
using Yaga.Reactive;
using Yaga.Test;
using Yaga.Test.Documentation;
using Object = UnityEngine.Object;

namespace Tests
{
    public class UiControlTest : BaseUiTest
    {
        [SetUp]
        public void SetUp()
        {
            UiBootstrap.InitializeSingleton();
            UiBootstrap.Instance.Bind<SimpleTextButtonView.Presenter>();
            UiBootstrap.Instance.Bind(new ObservablePresenter<ModelessView>());

            // I should do it because static fields are shared between tests and null is default value for reference types
            UiControl.Instance = null;
        }

        [Test]
        public void Create_WithoutParent_DefaultCanvasCreated()
        {
            UiControl.InitializeSingleton(Locator.canvasPrefab);
            var viewControl = UiControl.Instance.Create(Locator.modelessView);
            Assert.IsTrue(viewControl.View.transform.parent is object);

            var viewParent = viewControl.View.transform.parent.gameObject;
            Assert.IsTrue(viewParent.TryGetComponent<Canvas>(out _));
        }
        
        [Test]
        public void Create_WithParent_ViewIsChild()
        {
            UiControl.InitializeSingleton(Locator.canvasPrefab);
            var parent = Object.Instantiate(Locator.canvasPrefab).transform;
            var viewControl = UiControl.Instance.Create(Locator.modelessView, (RectTransform)parent);
            Assert.IsTrue(viewControl.View.transform.parent == parent);
        }

        [Test]
        public void Instance_BeforeInitialization_ThrowsException()
        {
            Assert.Throws<UiControlInitializationException>(() =>
            {
                var instance = UiControl.Instance;
            });
        }

        [Test]
        public void Create_WithModel_ModelShouldExists()
        {
            UiControl.InitializeSingleton(Locator.canvasPrefab);

            const string testModel = "testModel";
            var viewControl = UiControl.Instance.Create(Locator.simpleTextButtonView, testModel);
            Assert.IsTrue(((IView)viewControl.View).Model.HasValue);
            Assert.IsTrue(((IView<string>)viewControl.View).Model.HasValue);
            Assert.AreSame(testModel, ((IView<string>)viewControl.View).Model.ValueOrFailure().Model);
        }

        [Test]
        public void Create_UnitModel_ModelShouldExists()
        {
            UiControl.InitializeSingleton(Locator.canvasPrefab);

            var viewControl = UiControl.Instance.Create(Locator.modelessView);
            Assert.IsTrue(((IView)viewControl.View).Model.HasValue);
            Assert.IsTrue(((IView<Unit>)viewControl.View).Model.HasValue);
            Assert.AreEqual(Unit.Instance, ((IView<Unit>)viewControl.View).Model.ValueOrFailure().Model);
        }
        
        [Test]
        public void Create_NullPrefab_ThrowsException()
        {
            UiControl.InitializeSingleton(Locator.canvasPrefab);
            Assert.Throws<ArgumentNullException>(() => UiControl.Instance.Create((IView<Unit>)null));
        }
        [Test]
        public void Create_NullModel_ThrowsException()
        {
            UiControl.InitializeSingleton(Locator.canvasPrefab);
            Assert.Throws<ArgumentNullException>(() => UiControl.Instance.Create(Locator.simpleTextButtonView, default(string)));
        }
        [Test]
        public void Create_NullParent_ThrowsException()
        {
            UiControl.InitializeSingleton(Locator.canvasPrefab);
            Assert.Throws<ArgumentNullException>(() => UiControl.Instance.Create(Locator.simpleTextButtonView, "testModel", null));
        }
        [Test]
        public void Create_AlreadyCreatedView_ThrowsException()
        {
            UiControl.InitializeSingleton(Locator.canvasPrefab);
            var viewControl = UiControl.Instance.Create(Locator.simpleTextButtonView, "testModel");
            Assert.Throws<IsNotPrefabException>(() => UiControl.Instance.Create(viewControl.View, "testModel"));
        }
    }
}