using System.Collections;
using NUnit.Framework;
using UnityEngine.TestTools;
using Yaga;
using Yaga.Exceptions;
using Yaga.Test.Documentation;

namespace Tests.Documentation
{
    public class GettingStartedCodeTest : BaseUiTest
    {
        [UnityTest]
        public IEnumerator GettingStartedExample()
        {
            // Documentation example part.
            UiBootstrap.InitializeSingleton();
            UiControl.InitializeSingleton(Locator.canvasPrefab);

            UiBootstrap.Instance.Bind<SimpleTextButtonView.Presenter>();
            var viewControl = UiControl.Instance.Create(Locator.simpleTextButtonView, "Sample text");
            yield return null;

            // Testing part.
            var wasInvoked = false;
            Assert.AreEqual("Sample text", viewControl.View.Text.text);
            viewControl.Subs.MatchSome(subs => subs.Subscribe(viewControl.View.Button, () => wasInvoked = true));
            viewControl.View.Button.onClick.Invoke();
            Assert.IsTrue(wasInvoked, "Button action was not executed");

            // Again documentation example part.
            viewControl.Set("New text");

            // Testing part.
            wasInvoked = false;
            Assert.AreEqual("New text", viewControl.View.Text.text);
            viewControl.Subs.MatchSome(subs => subs.Subscribe(viewControl.View.Button, () => wasInvoked = true));
            viewControl.View.Button.onClick.Invoke();
            Assert.IsTrue(wasInvoked, "Button action was not executed");

            // Documentation example part.
            viewControl.Unset();

            // Testing part.
            wasInvoked = false;
            // It will not match, because Subs is None.
            viewControl.Subs.MatchSome(subs => subs.Subscribe(viewControl.View.Button, () => wasInvoked = true));
            Assert.IsFalse(viewControl.Subs.HasValue);
            viewControl.View.Button.onClick.Invoke();
            Assert.IsFalse(wasInvoked, "Button action was executed");

            // Documentation example part.
            viewControl.View.Destroy();

            // Testing part.
            yield return null;
            // GameObject is destroyed, so viewControl.View is null.
            Assert.IsTrue(viewControl.View == null);
        }
    }
}