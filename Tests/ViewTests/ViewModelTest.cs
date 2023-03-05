using System;
using NUnit.Framework;
using Tests.Presenters;
using Yaga;
using Yaga.Exceptions;
using Yaga.Test;
using Yaga.Test.Documentation;
using Object = UnityEngine.Object;

namespace Tests.ViewTests
{
    public class ViewModelTest : BaseUiTest
    {
        [SetUp]
        public void SetUp()
        {
            UiBootstrap.InitializeSingleton();
            UiControl.InitializeSingleton(Locator.canvasPrefab);
        }

        [Test]
        public void ModelSet()
        {
            const string testModel = "testModel";
            UiBootstrap.Bind<SimpleTextButtonView.Presenter>();
            var view = UiControl.Instance.Create(Locator.simpleTextButtonView, testModel);
            Assert.AreSame(testModel, view.Model);
            Assert.IsTrue(view.HasModel);
        }

        [Test]
        public void ModelNullSet()
        {
            const string testModel = "testModel";
            UiBootstrap.Bind<SimpleTextButtonView.Presenter>();
            var view = UiControl.Instance.Create(Locator.simpleTextButtonView, testModel);
            Assert.Catch<ArgumentNullException>(() => view.Set(null));
        }

        [Test]
        public void ModelAfterChange()
        {
            const string testModel = "testModel";
            UiBootstrap.Bind<SimpleTextButtonView.Presenter>();
            var view = UiControl.Instance.Create(Locator.simpleTextButtonView, "oldModel");
            view.Set(testModel);
            Assert.AreSame(testModel, view.Model);
        }

        [Test]
        public void HasModelOnViewWithoutSetModel()
        {
            UiBootstrap.Bind<SimpleTextButtonView.Presenter>();
            var view = Object.Instantiate(Locator.simpleTextButtonView);
            Assert.IsFalse(view.HasModel);
        }

        [Test]
        public void HasModelOnViewAfterUnset()
        {
            const string testModel = "testModel";
            UiBootstrap.Bind<SimpleTextButtonView.Presenter>();
            var view = UiControl.Instance.Create(Locator.simpleTextButtonView, testModel);
            view.Unset();
            Assert.IsFalse(view.HasModel);
        }

        [Test]
        public void ExceptionOnGettingNotSetModel()
        {
            UiBootstrap.Bind<SimpleTextButtonView.Presenter>();
            var view = Object.Instantiate(Locator.simpleTextButtonView);
            Assert.Catch<ModelIsNotSetException>(() =>
            {
                var value = view.Model;
            });
        }

        [Test]
        public void SameModelSet()
        {
            var invokedSet = false;
            const string testModel = "testModel";
            UiBootstrap.Bind(new TestPresenter(_ => invokedSet = true));
            var view = UiControl.Instance.Create(Locator.simpleTextButtonView, testModel);
            Assert.IsTrue(invokedSet);
            invokedSet = false;
            view.Set(testModel);
            Assert.IsFalse(invokedSet);
        }

        private class TestPresenter : ObservablePresenter<SimpleTextButtonView, string>
        {
            public TestPresenter(Action<SimpleTextButtonView> onModelSet = null,
                Action<SimpleTextButtonView> onModelUnset = null) : base(onModelSet, onModelUnset)
            {
            }
        }
    }
}