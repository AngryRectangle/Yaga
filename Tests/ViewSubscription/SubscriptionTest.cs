using System;
using System.Linq;
using NUnit.Framework;
using Tests.Presenters;
using Yaga;
using Yaga.Extensions;
using Yaga.Test;
using Yaga.Test.Documentation;

namespace Tests.ViewSubscription
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
        public void EventSubscription()
        {
            var invoked = false;
            var testAction = new Action(() => { });
            var presenter = new TestPresenter(view =>
                view.SubscribeEvent(ref testAction, action => testAction -= action, () => invoked = true));
            UiBootstrap.Bind(presenter);
            var view = UiControl.Instance.Create(Locator.simpleTextButtonView, "Sample text");
            Assert.False(invoked);
            testAction.Invoke();
            Assert.True(invoked);
        }

        [Test]
        public void EventUnsubscription()
        {
            var invoked = false;
            var testAction = new Action(() => { });
            var presenter = new TestPresenter(view =>
                view.SubscribeEvent(ref testAction, action => testAction -= action, () => invoked = true));
            UiBootstrap.Bind(presenter);
            var view = UiControl.Instance.Create(Locator.simpleTextButtonView, "Sample text");
            Assert.False(invoked);
            view.Unset();
            testAction.Invoke();
            Assert.False(invoked);
        }

        [Test]
        public void CustomEventUnsubscription()
        {
            var invoked = false;
            var testAction = new Action(() => { });
            var presenter = new TestPresenter(view =>
                view.SubscribeEvent<Action>(action => testAction += action, action => testAction -= action,
                    () => invoked = true));
            UiBootstrap.Bind(presenter);
            var view = UiControl.Instance.Create(Locator.simpleTextButtonView, "Sample text");
            Assert.False(invoked);
            view.Unset();
            testAction.Invoke();
            Assert.False(invoked);
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

            UiBootstrap.Bind(presenterWithModel);
            UiBootstrap.Bind(presenterWithoutModel);
            UiBootstrap.Bind<ChildrenViewPresenter>();

            var view = UiControl.Instance.Create(Locator.viewWithChild);

            Assert.True(setOnViewWithModel);
            Assert.True(setOnViewWithoutModel);

            UiBootstrap.Instance.Unset(view);

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
            protected override void OnSet(ViewWithChild view)
            {
                UiBootstrap.Instance.Set(view.modelessView);
                view.viewWithModel.Set("text");
            }
        }
    }
}