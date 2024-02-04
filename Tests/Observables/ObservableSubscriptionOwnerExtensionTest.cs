using System;
using NUnit.Framework;
using Tests.Presenters;
using Yaga;
using Yaga.Reactive;
using Yaga.Test;
using Yaga.Test.Documentation;

namespace Tests.Observables
{
    public class ObservableSubscriptionOwnerExtensionTest : BaseUiTest
    {
        [SetUp]
        public void SetUp()
        {
            UiBootstrap.InitializeSingleton();
            UiControl.InitializeSingleton(Locator.canvasPrefab);
        }

        [Test]
        public void Set_InitialValue()
        {
            var observableText = new Observable<string>("initial");
            var presenter = new ChildrenViewPresenter(observableText);
            UiBootstrap.Instance.Bind(presenter);
            UiBootstrap.Instance.Bind<SimpleTextButtonView.Presenter>();
            UiBootstrap.Instance.Bind<ObservablePresenter<ModelessView>>();
            
            var view = UiControl.Instance.Create(Locator.viewWithChild);
            
            Assert.AreEqual("initial", view.View.viewWithModel.Text.text);
        }
        
        [Test]
        public void Set_InitialValue_ParentUnset()
        {
            var observableText = new Observable<string>("initial");
            var presenter = new ChildrenViewPresenter(observableText);
            UiBootstrap.Instance.Bind(presenter);
            UiBootstrap.Instance.Bind<SimpleTextButtonView.Presenter>();
            UiBootstrap.Instance.Bind<ObservablePresenter<ModelessView>>();
            
            var view = UiControl.Instance.Create(Locator.viewWithChild);
            view.Unset();
            observableText.Value = "changed";
            
            // Unsubscribed, so the value should not change
            Assert.AreEqual("initial", view.View.viewWithModel.Text.text);
        }
        
        [Test]
        public void Set_InitialValue_Unsubscribed()
        {
            var observableText = new Observable<string>("initial");
            var presenter = new ChildrenViewPresenter(observableText);
            UiBootstrap.Instance.Bind(presenter);
            UiBootstrap.Instance.Bind<SimpleTextButtonView.Presenter>();
            UiBootstrap.Instance.Bind<ObservablePresenter<ModelessView>>();
            
            var view = UiControl.Instance.Create(Locator.viewWithChild);
            presenter.unsubscriptionKey.Dispose();
            observableText.Value = "changed";
            
            // Unsubscribed, so the value should not change
            Assert.AreEqual("initial", view.View.viewWithModel.Text.text);
        }

        [Test]
        public void Set_ChangeValue()
        {
            var observableText = new Observable<string>("initial");
            var presenter = new ChildrenViewPresenter(observableText);
            UiBootstrap.Instance.Bind(presenter);
            UiBootstrap.Instance.Bind<SimpleTextButtonView.Presenter>();
            UiBootstrap.Instance.Bind<ObservablePresenter<ModelessView>>();
            
            var view = UiControl.Instance.Create(Locator.viewWithChild);
            
            observableText.Value = "changed";
            Assert.AreEqual("changed", view.View.viewWithModel.Text.text);
        }
        
        [Test]
        public void Set_ChangeValue_Unsubscribed()
        {
            var observableText = new Observable<string>("initial");
            var presenter = new ChildrenViewPresenter(observableText);
            UiBootstrap.Instance.Bind(presenter);
            UiBootstrap.Instance.Bind<SimpleTextButtonView.Presenter>();
            UiBootstrap.Instance.Bind<ObservablePresenter<ModelessView>>();
            
            var view = UiControl.Instance.Create(Locator.viewWithChild);
            observableText.Value = "changed";
            presenter.unsubscriptionKey.Dispose();
            observableText.Value = "again";
            
            // Unsubscribed, so the value should not change
            Assert.AreEqual("changed", view.View.viewWithModel.Text.text);
        }
        
        [Test]
        public void Set_ChangeValue_ParentUnset()
        {
            var observableText = new Observable<string>("initial");
            var presenter = new ChildrenViewPresenter(observableText);
            UiBootstrap.Instance.Bind(presenter);
            UiBootstrap.Instance.Bind<SimpleTextButtonView.Presenter>();
            UiBootstrap.Instance.Bind<ObservablePresenter<ModelessView>>();
            
            var view = UiControl.Instance.Create(Locator.viewWithChild);
            observableText.Value = "changed";
            view.Unset();
            observableText.Value = "again";
            
            // Unsubscribed, so the value should not change
            Assert.AreEqual("changed", view.View.viewWithModel.Text.text);
        }
        
        private class ChildrenViewPresenter : Presenter<ViewWithChild>
        {
            private readonly IReadOnlyObservable<string> _observableText;
            public IDisposable unsubscriptionKey;

            public ChildrenViewPresenter(IReadOnlyObservable<string> observableText)
            {
                _observableText = observableText;
            }

            protected override void OnSet(ViewWithChild view, ISubscriptions subs)
            {
                unsubscriptionKey = subs.Set(view.viewWithModel, _observableText);
            }
        }
    }
}