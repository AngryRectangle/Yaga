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
            UiBootstrap.Bind<SimpleTextButtonView.Presenter>();
            var viewControl = UiControl.Instance.Create(Locator.simpleTextButtonView, "oldModel");
            viewControl.Set(testModel);

            Assert.AreSame(testModel, ((IView<string>)viewControl.View).Model.ValueOrFailure().Model);
        }

        [Test]
        public void Unset_ModelUnset()
        {
            const string testModel = "testModel";
            UiBootstrap.Bind<SimpleTextButtonView.Presenter>();
            var viewControl = UiControl.Instance.Create(Locator.simpleTextButtonView, testModel);
            viewControl.Unset();

            Assert.IsFalse(((IView)viewControl.View).Model.HasValue);
            Assert.IsFalse(((IView<string>)viewControl.View).Model.HasValue);
        }
        
        [Test]
        public void Add_AfterUnset_ThrowsException()
        {
            const string testModel = "testModel";
            UiBootstrap.Bind<SimpleTextButtonView.Presenter>();
            var viewControl = UiControl.Instance.Create(Locator.simpleTextButtonView, testModel);
            viewControl.Unset();

            Assert.Throws<ViewModelIsUnsetException>(() => viewControl.Add(new Disposable()));
        }
        
        [Test]
        public void Add_ActuallyAdds()
        {
            var isInvoked = false;
            const string testModel = "testModel";
            UiBootstrap.Bind<SimpleTextButtonView.Presenter>();
            var viewControl = UiControl.Instance.Create(Locator.simpleTextButtonView, testModel);
            viewControl.Add(new Disposable(()=>isInvoked = true));
            viewControl.Unset();
            
            Assert.IsTrue(isInvoked);
        }
        
        [Test]
        public void Remove_ActuallyRemoves()
        {
            var isInvoked = false;
            const string testModel = "testModel";
            UiBootstrap.Bind<SimpleTextButtonView.Presenter>();
            var viewControl = UiControl.Instance.Create(Locator.simpleTextButtonView, testModel);
            var key = viewControl.Add(new Disposable(()=>isInvoked = true));
            var isRemoved = viewControl.Remove(key);
            viewControl.Unset();
            
            Assert.IsTrue(isRemoved);
            Assert.IsFalse(isInvoked);
        }
        
        [Test]
        public void Remove_IncorrectKey_ReturnsFalse()
        {
            const string testModel = "testModel";
            UiBootstrap.Bind<SimpleTextButtonView.Presenter>();
            var viewControl = UiControl.Instance.Create(Locator.simpleTextButtonView, testModel);
            var isRemoved = viewControl.Remove(new ISubscriptionsOwner.Key());
            Assert.IsFalse(isRemoved);
        }
        
        [Test]
        public void Remove_AfterUnset_ThrowsException()
        {
            const string testModel = "testModel";
            UiBootstrap.Bind<SimpleTextButtonView.Presenter>();
            var viewControl = UiControl.Instance.Create(Locator.simpleTextButtonView, testModel);
            viewControl.Unset();

            Assert.Throws<ViewModelIsUnsetException>(() => viewControl.Remove(new ISubscriptionsOwner.Key()));
        }
    }
}