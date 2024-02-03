using System.Collections;
using NUnit.Framework;
using UnityEngine.TestTools;
using Yaga;
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
            Assert.AreEqual("Sample text", viewControl.View.Text.text);
            viewControl.Subscribe(viewControl.View.Button, Assert.Pass);
            viewControl.View.Button.onClick.Invoke();
            Assert.Fail("Button action was not executed");
        }
    }
}