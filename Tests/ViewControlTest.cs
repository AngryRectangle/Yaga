using NUnit.Framework;
using Optional.Unsafe;
using Yaga;
using Yaga.Exceptions;
using Yaga.Reactive;
using Yaga.Test.Documentation;

namespace Tests
{
    public class ViewControlTest : BaseUiTest
    {
        [SetUp]
        public void SetUp()
        {
            UiBootstrap.InitializeSingleton();
            UiControl.InitializeSingleton(Locator.canvasPrefab);
        }

        [Test]
        public void Set_ModelUpdated()
        {
            const string testModel = "testModel";
            UiBootstrap.Instance.Bind<SimpleTextButtonView.Presenter>();
            var viewControl = UiControl.Instance.Create(Locator.simpleTextButtonView, "oldModel");
            viewControl.Set(testModel);

            Assert.AreSame(testModel, ((IView<string>)viewControl.View).Model.ValueOrFailure().Model);
        }

        [Test]
        public void Unset_ModelUnset()
        {
            const string testModel = "testModel";
            UiBootstrap.Instance.Bind<SimpleTextButtonView.Presenter>();
            var viewControl = UiControl.Instance.Create(Locator.simpleTextButtonView, testModel);
            viewControl.Unset();

            Assert.IsFalse(((IView)viewControl.View).Model.HasValue);
            Assert.IsFalse(((IView<string>)viewControl.View).Model.HasValue);
        }

        [Test]
        public void Subs_ModelSet_IsSome()
        {
            const string testModel = "testModel";
            UiBootstrap.Instance.Bind<SimpleTextButtonView.Presenter>();
            var viewControl = UiControl.Instance.Create(Locator.simpleTextButtonView, testModel);
            
            Assert.IsTrue(viewControl.Subs.HasValue);
        }
        
        [Test]
        public void Subs_ModelUnset_IsNone()
        {
            const string testModel = "testModel";
            UiBootstrap.Instance.Bind<SimpleTextButtonView.Presenter>();
            var viewControl = UiControl.Instance.Create(Locator.simpleTextButtonView, testModel);
            viewControl.Unset();
            
            Assert.IsFalse(viewControl.Subs.HasValue);
        }
    }
}