using NUnit.Framework;
using Yaga;

namespace Tests.Examples
{
    public class RegistrationFormExampleTest : BaseUiTest
    {
        [SetUp]
        public void SetUp()
        {
            UiBootstrap.InitializeSingleton();
            UiControl.InitializeSingleton(Locator.canvasPrefab);
            
            UiBootstrap.Instance.Bind(new RegistrationFormWindowView.Presenter());
        }
        
        [Test]
        public void UsernamePasswordInput_PasswordTooShort()
        {
            var model = new RegistrationFormWindowView.Model();
            var viewControl = UiControl.Instance.Create(Locator.registrationFormWindowView, model);
            
            viewControl.View._usernameInput.text = "TestUser";
            viewControl.View._passwordInput.text = "short";
            
            Assert.IsTrue(viewControl.View._errorText.gameObject.activeSelf);
            Assert.AreEqual("Password is too short.", viewControl.View._errorText.text);
        }
        
        [Test]
        public void UsernamePasswordInput_SubmitClicked()
        {
            var model = new RegistrationFormWindowView.Model();
            var viewControl = UiControl.Instance.Create(Locator.registrationFormWindowView, model);
            bool submitCalled = false;
            model.OnSubmit.Add(data =>
            {
                submitCalled = true;
                Assert.AreEqual("TestUser", data.Username);
                Assert.AreEqual("ValidPass123", data.Password);
            });
            
            viewControl.View._usernameInput.text = "TestUser";
            viewControl.View._passwordInput.text = "ValidPass123";
            viewControl.View._submitButton.onClick.Invoke();
            
            Assert.IsTrue(viewControl.View._submitButton.interactable);
            Assert.IsTrue(submitCalled);
        }
    }
}