using System;
using NUnit.Framework;
using Tests.Presenters;
using Yaga;
using Yaga.Test;
using Yaga.Test.Documentation;

namespace Tests
{
    public class SubscriptionTest : BaseUiTest
    {
        [SetUp]
        public void SetUp()
        {
            UiBootstrap.InitializeSingleton();
            UiControl.InitializeSingleton(Locator.canvasPrefab);
        }

        [Test]
        public void ModelUnsetOnChildren()
        {
            var unsetOnViewWithModel = false;
            var setOnViewWithModel = false;
            var unsetOnViewWithoutModel = false;
            var setOnViewWithoutModel = false;
            var presenterWithModel =
                new TestPresenter(_ => setOnViewWithModel = true, _ => unsetOnViewWithModel = true);
            var presenterWithoutModel =
                new ModelessPresenter(_ => setOnViewWithoutModel = true, _ => unsetOnViewWithoutModel = true);

            UiBootstrap.Instance.Bind(presenterWithModel);
            UiBootstrap.Instance.Bind(presenterWithoutModel);
            UiBootstrap.Instance.Bind<ChildrenViewPresenter>();

            var viewControl = UiControl.Instance.Create(Locator.viewWithChild);

            Assert.True(setOnViewWithModel);
            Assert.True(setOnViewWithoutModel);

            viewControl.Unset();

            Assert.True(unsetOnViewWithModel);
            Assert.True(unsetOnViewWithoutModel);
        }

        private class TestPresenter : ObservablePresenter<SimpleTextButtonView, string>
        {
            public TestPresenter(Action<SimpleTextButtonView> onModelSet = null,
                Action<SimpleTextButtonView> onModelUnset = null) : base(onModelSet, onModelUnset)
            {
            }
        }

        private class ModelessPresenter : ObservableModelessPresenter<ModelessView>
        {
            public ModelessPresenter(Action<ModelessView> onModelSet = null, Action<ModelessView> onModelUnset = null) :
                base(onModelSet, onModelUnset)
            {
            }
        }

        private class ChildrenViewPresenter : Presenter<ViewWithChild>
        {
            protected override void OnSet(ViewWithChild view, ISubscriptionsOwner subs)
            {
                subs.Set(view.modelessView);
                subs.Set(view.viewWithModel, "text");
            }
        }
    }
}