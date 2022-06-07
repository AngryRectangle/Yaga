using System.Collections;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace Yaga.Test.Documentation
{
    public class GettingStartedCodeTest : BaseUiTest
    {
        [UnityTest]
        public IEnumerator GettingStartedExample()
        {
            // Documentation example part.
            UiBootstrap.InitializeSingleton();
            UiControl.InitializeSingleton(Locator.canvasPrefab);

            UiBootstrap.Bind<SimpleTextButtonView.Presenter>();
            var instance = UiControl.Instance.Create(Locator.simpleTextButtonView, "Sample text");
            yield return null;

            // Testing part.
            Assert.AreEqual("Sample text", instance.Text.text);
            instance.Subscribe(instance.Button, Assert.Pass);
            instance.Button.onClick.Invoke();
            Assert.Fail("Button action was not executed");
        }
    }
}