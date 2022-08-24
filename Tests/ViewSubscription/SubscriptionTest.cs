using System;
using NUnit.Framework;
using Yaga;
using Yaga.Extensions;
using Yaga.Test;
using Yaga.Test.Documentation;

namespace Tests.ViewSubscription
{
    public class SubscriptionTest : BaseUiTest
    {
        [Test]
        public void EventSubscription()
        {
            UiBootstrap.InitializeSingleton();
            UiControl.InitializeSingleton(Locator.canvasPrefab);

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
            UiBootstrap.InitializeSingleton();
            UiControl.InitializeSingleton(Locator.canvasPrefab);

            var invoked = false;
            var testAction = new Action(() => { });
            var presenter = new TestPresenter(view =>
                view.SubscribeEvent(ref testAction, action => testAction -= action, () => invoked = true));
            UiBootstrap.Bind(presenter);
            var view = UiControl.Instance.Create(Locator.simpleTextButtonView, "Sample text");
            Assert.False(invoked);
            view.Unset("OnClick");
            testAction.Invoke();
            Assert.False(invoked);
        }

        [Test]
        public void CustomEventUnsubscription()
        {
            UiBootstrap.InitializeSingleton();
            UiControl.InitializeSingleton(Locator.canvasPrefab);

            var invoked = false;
            var testAction = new Action(() => { });
            var presenter = new TestPresenter(view =>
                view.SubscribeEvent<Action>(action => testAction += action, action => testAction -= action,
                    () => invoked = true));
            UiBootstrap.Bind(presenter);
            var view = UiControl.Instance.Create(Locator.simpleTextButtonView, "Sample text");
            Assert.False(invoked);
            view.Unset("OnClick");
            testAction.Invoke();
            Assert.False(invoked);
        }


        private class TestPresenter : Presenter<SimpleTextButtonView, string>
        {
            private readonly Action<View> _onModelSet;

            public TestPresenter(Action<View> onModelSet)
            {
                _onModelSet = onModelSet;
            }

            protected override void OnModelSet(SimpleTextButtonView view, string model)
            {
                _onModelSet(view);
            }
        }
    }
}