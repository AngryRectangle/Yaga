using System;
using System.Collections;
using NUnit.Framework;
using Optional;
using Tests.Presenters;
using UnityEngine.TestTools;
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
        public void Set_Observable_InitialValue()
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
        public void Set_Observable_InitialValue_ParentUnset()
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
        public void Set_Observable_InitialValue_Unsubscribed()
        {
            var observableText = new Observable<string>("initial");
            var presenter = new ChildrenViewPresenter(observableText);
            UiBootstrap.Instance.Bind(presenter);
            UiBootstrap.Instance.Bind<SimpleTextButtonView.Presenter>();
            UiBootstrap.Instance.Bind<ObservablePresenter<ModelessView>>();

            var view = UiControl.Instance.Create(Locator.viewWithChild);
            presenter.unsubscription.Dispose();
            observableText.Value = "changed";

            // Unsubscribed, so the value should not change
            Assert.AreEqual("initial", view.View.viewWithModel.Text.text);
        }

        [Test]
        public void Set_Observable_ChangeValue()
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
        
        [UnityTest]
        public IEnumerator Set_Observable_DestroySubscribedView()
        {
            var observableText = new Observable<string>("initial");
            var presenter = new ChildrenViewPresenter(observableText);
            UiBootstrap.Instance.Bind(presenter);
            UiBootstrap.Instance.Bind<SimpleTextButtonView.Presenter>();
            UiBootstrap.Instance.Bind<ObservablePresenter<ModelessView>>();

            var view = UiControl.Instance.Create(Locator.viewWithChild);

            view.View.viewWithModel.Destroy();
            yield return null;
            
            observableText.Value = "changed";
            // No exceptions is fine too.
        }

        [Test]
        public void Set_Observable_ChangeValue_Unsubscribed()
        {
            var observableText = new Observable<string>("initial");
            var presenter = new ChildrenViewPresenter(observableText);
            UiBootstrap.Instance.Bind(presenter);
            UiBootstrap.Instance.Bind<SimpleTextButtonView.Presenter>();
            UiBootstrap.Instance.Bind<ObservablePresenter<ModelessView>>();

            var view = UiControl.Instance.Create(Locator.viewWithChild);
            observableText.Value = "changed";
            presenter.unsubscription.Dispose();
            observableText.Value = "again";

            // Unsubscribed, so the value should not change
            Assert.AreEqual("changed", view.View.viewWithModel.Text.text);
        }

        [Test]
        public void Set_Observable_ChangeValue_ParentUnset()
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
            public IDisposable unsubscription;

            public ChildrenViewPresenter(IReadOnlyObservable<string> observableText)
            {
                _observableText = observableText;
            }

            protected override void OnSet(ViewWithChild view, ISubscriptions subs)
            {
                unsubscription = subs.Set(view.viewWithModel, _observableText);
            }
        }
    }
}