using NUnit.Framework;
using Optional;
using TMPro;
using UnityEngine;
using Yaga;
using Yaga.Reactive.ObservableExtensions;
using Yaga.Test.Documentation;

namespace Tests.Observables
{
    public class ObservableUiExtensionTest : BaseUiTest
    {
        [SetUp]
        public void SetUp()
        {
            UiBootstrap.InitializeSingleton();
            UiBootstrap.Instance.Bind<SimpleTextButtonView.Presenter>();

            UiControl.InitializeSingleton(Locator.canvasPrefab);
        }

        [Test]
        public void IntoActive_GameObject_Subscribe_Init()
        {
            var testGo = new GameObject();
            var observable = new Yaga.Reactive.Observable<bool>(false);

            var disposable = observable.IntoActive(testGo);

            Assert.IsFalse(testGo.activeSelf);
        }

        [Test]
        public void IntoActive_GameObject_Subscribe_Reactive()
        {
            var testGo = new GameObject();
            var observable = new Yaga.Reactive.Observable<bool>(false);

            var disposable = observable.IntoActive(testGo);
            observable.Value = true;

            Assert.IsTrue(testGo.activeSelf);
        }

        [Test]
        public void IntoActive_GameObject_Unsubscribe()
        {
            var testGo = new GameObject();
            var observable = new Yaga.Reactive.Observable<bool>(false);
            var disposable = observable.IntoActive(testGo);

            disposable.Dispose();
            observable.Value = true;

            Assert.IsFalse(testGo.activeSelf);
        }

        [Test]
        public void Into_TMP_Text_Subscribe_Init()
        {
            var testGo = new GameObject();
            var text = testGo.AddComponent<TextMeshProUGUI>();
            var testString = "Initial";
            var observable = new Yaga.Reactive.Observable<string>(testString);

            var disposable = observable.Into(text);

            Assert.AreEqual(testString, text.text);
        }

        [Test]
        public void Into_TMP_Text_Subscribe_Reactive()
        {
            var testGo = new GameObject();
            var text = testGo.AddComponent<TextMeshProUGUI>();
            var testString = "Initial";
            var observable = new Yaga.Reactive.Observable<string>(testString);

            var disposable = observable.Into(text);
            var newString = "Changed";
            observable.Value = newString;

            Assert.AreEqual(newString, text.text);
        }

        [Test]
        public void Into_TMP_Text_Unsubscribe()
        {
            var testGo = new GameObject();
            var text = testGo.AddComponent<TextMeshProUGUI>();
            var testString = "Initial";
            var observable = new Yaga.Reactive.Observable<string>(testString);
            var disposable = observable.Into(text);

            disposable.Dispose();
            var newString = "Changed";
            observable.Value = newString;

            Assert.AreEqual(testString, text.text);
        }

        [Test]
        public void Into_Generic_TMP_Text_Subscribe_Init()
        {
            var testGo = new GameObject();
            var text = testGo.AddComponent<TextMeshProUGUI>();
            var testValue = 42;
            var observable = new Yaga.Reactive.Observable<int>(testValue);

            var disposable = observable.Into(text);

            Assert.AreEqual("42", text.text);
        }

        [Test]
        public void Into_Generic_TMP_Text_Subscribe_Reactive()
        {
            var testGo = new GameObject();
            var text = testGo.AddComponent<TextMeshProUGUI>();
            var testValue = 42;
            var observable = new Yaga.Reactive.Observable<int>(testValue);

            var disposable = observable.Into(text);
            var newValue = 84;
            observable.Value = newValue;

            Assert.AreEqual("84", text.text);
        }

        [Test]
        public void Into_Generic_TMP_Text_Unsubscribe()
        {
            var testGo = new GameObject();
            var text = testGo.AddComponent<TextMeshProUGUI>();
            var testValue = 42;
            var observable = new Yaga.Reactive.Observable<int>(testValue);
            var disposable = observable.Into(text);

            disposable.Dispose();
            var newValue = 84;
            observable.Value = newValue;

            Assert.AreEqual("42", text.text);
        }

        [Test]
        public void Into_View_Subscribe_Init()
        {
            var testString = "Initial";
            var observable = new Yaga.Reactive.Observable<string>(testString);
            var viewControl = UiControl.Instance.Create(Locator.simpleTextButtonView, "no");

            var disposable = observable.Into(viewControl.View);

            Assert.AreEqual(testString, viewControl.View.Text.text);
        }

        [Test]
        public void Into_View_Subscribe_Reactive()
        {
            var testString = "Initial";
            var observable = new Yaga.Reactive.Observable<string>(testString);
            var viewControl = UiControl.Instance.Create(Locator.simpleTextButtonView, "no");

            var disposable = observable.Into(viewControl.View);
            var newString = "Changed";
            observable.Value = newString;

            Assert.AreEqual(newString, viewControl.View.Text.text);
        }

        [Test]
        public void Into_View_Unsubscribe()
        {
            var testString = "Initial";
            var observable = new Yaga.Reactive.Observable<string>(testString);
            var viewControl = UiControl.Instance.Create(Locator.simpleTextButtonView, "no");
            var disposable = observable.Into(viewControl.View);

            disposable.Dispose();
            var newString = "Changed";
            observable.Value = newString;

            Assert.AreEqual(testString, viewControl.View.Text.text);
        }

        [Test]
        public void Into_View_NoErrorsAfterViewDestroy_Reactive()
        {
            var testString = "Initial";
            var observable = new Yaga.Reactive.Observable<string>(testString);
            var viewControl = UiControl.Instance.Create(Locator.simpleTextButtonView, "no");

            var disposable = observable.Into(viewControl.View);
            Object.DestroyImmediate(viewControl.View.gameObject);
            var newString = "Changed";
            observable.Value = newString;

            Assert.Pass();
        }

        [Test]
        public void Into_View_NoErrorsAfterViewDestroy_Unsubscribe()
        {
            var testString = "Initial";
            var observable = new Yaga.Reactive.Observable<string>(testString);
            var viewControl = UiControl.Instance.Create(Locator.simpleTextButtonView, "no");

            var disposable = observable.Into(viewControl.View);
            Object.DestroyImmediate(viewControl.View.gameObject);
            disposable.Dispose();

            Assert.Pass();
        }

        [Test]
        public void Into_Option_View_Subscribe_Init()
        {
            var testString = "Initial";
            var observable = new Yaga.Reactive.Observable<Option<string>>(Option.None<string>());
            var viewControl = UiControl.Instance.Create(Locator.simpleTextButtonView, "no");

            var disposable = observable.Into(viewControl.View);

            Assert.IsFalse(viewControl.View.gameObject.activeSelf);
        }

        [Test]
        public void Into_Option_View_Subscribe_Reactive_Some()
        {
            var testString = "Initial";
            var observable = new Yaga.Reactive.Observable<Option<string>>(Option.None<string>());
            var viewControl = UiControl.Instance.Create(Locator.simpleTextButtonView, "no");

            var disposable = observable.Into(viewControl.View);
            observable.Value = Option.Some(testString);

            Assert.IsTrue(viewControl.View.gameObject.activeSelf);
            Assert.AreEqual(testString, viewControl.View.Text.text);
        }

        [Test]
        public void Into_Option_View_Subscribe_Reactive_None()
        {
            var testString = "Initial";
            var observable = new Yaga.Reactive.Observable<Option<string>>(Option.Some(testString));
            var viewControl = UiControl.Instance.Create(Locator.simpleTextButtonView, "no");

            var disposable = observable.Into(viewControl.View);
            observable.Value = Option.None<string>();

            Assert.IsFalse(viewControl.View.gameObject.activeSelf);
        }

        [Test]
        public void Into_Option_View_Unsubscribe()
        {
            var testString = "Initial";
            var observable = new Yaga.Reactive.Observable<Option<string>>(Option.Some(testString));
            var viewControl = UiControl.Instance.Create(Locator.simpleTextButtonView, "no");
            var disposable = observable.Into(viewControl.View);

            disposable.Dispose();
            observable.Value = Option.None<string>();

            Assert.IsTrue(viewControl.View.gameObject.activeSelf);
            Assert.AreEqual(testString, viewControl.View.Text.text);
        }

        [Test]
        public void Into_Option_View_NoErrorsAfterViewDestroy_Reactive()
        {
            var testString = "Initial";
            var observable = new Yaga.Reactive.Observable<Option<string>>(Option.None<string>());
            var viewControl = UiControl.Instance.Create(Locator.simpleTextButtonView, "no");

            var disposable = observable.Into(viewControl.View);
            Object.DestroyImmediate(viewControl.View.gameObject);
            observable.Value = Option.Some(testString);

            Assert.Pass();
        }

        [Test]
        public void Into_Option_View_NoErrorsAfterViewDestroy_Unsubscribe()
        {
            var testString = "Initial";
            var observable = new Yaga.Reactive.Observable<Option<string>>(Option.Some(testString));
            var viewControl = UiControl.Instance.Create(Locator.simpleTextButtonView, "no");

            var disposable = observable.Into(viewControl.View);
            Object.DestroyImmediate(viewControl.View.gameObject);
            disposable.Dispose();

            Assert.Pass();
        }
    }
}