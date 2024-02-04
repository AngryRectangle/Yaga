using System;
using NUnit.Framework;
using Optional;
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

        [Test]
        public void Set_OptionalObservable_InitialValue_Some_NothingStrategy()
        {
            var observableText = new OptionalObservable<string>("initial");
            var presenter = new ChildrenViewOptionalPresenter(observableText, ObservableSubscriptionOwnerExtension.OptionalStrategy.Nothing);
            UiBootstrap.Instance.Bind(presenter);
            UiBootstrap.Instance.Bind<SimpleTextButtonView.Presenter>();
            UiBootstrap.Instance.Bind<ObservablePresenter<ModelessView>>();

            var view = UiControl.Instance.Create(Locator.viewWithChild);

            Assert.AreEqual("initial", view.View.viewWithModel.Text.text);
            Assert.IsTrue(view.View.viewWithModel.gameObject.activeSelf);
        }

        [Test]
        public void Set_OptionalObservable_InitialValue_Some_ActivityStrategy()
        {
            var observableText = new OptionalObservable<string>("initial");
            var presenter = new ChildrenViewOptionalPresenter(observableText, ObservableSubscriptionOwnerExtension.OptionalStrategy.Activity);
            UiBootstrap.Instance.Bind(presenter);
            UiBootstrap.Instance.Bind<SimpleTextButtonView.Presenter>();
            UiBootstrap.Instance.Bind<ObservablePresenter<ModelessView>>();

            var view = UiControl.Instance.Create(Locator.viewWithChild);

            Assert.AreEqual("initial", view.View.viewWithModel.Text.text);
            Assert.IsTrue(view.View.viewWithModel.gameObject.activeSelf);
        }
        
        [Test]
        public void Set_OptionalObservable_InitialValue_None_NothingStrategy()
        {
            var observableText = new OptionalObservable<string>();
            var presenter = new ChildrenViewOptionalPresenter(observableText, ObservableSubscriptionOwnerExtension.OptionalStrategy.Nothing);
            UiBootstrap.Instance.Bind(presenter);
            UiBootstrap.Instance.Bind<SimpleTextButtonView.Presenter>();
            UiBootstrap.Instance.Bind<ObservablePresenter<ModelessView>>();

            var view = UiControl.Instance.Create(Locator.viewWithChild);

            Assert.IsTrue(view.View.viewWithModel.gameObject.activeSelf);
        }

        [Test]
        public void Set_OptionalObservable_InitialValue_None_ActivityStrategy()
        {
            var observableText = new OptionalObservable<string>();
            var presenter = new ChildrenViewOptionalPresenter(observableText, ObservableSubscriptionOwnerExtension.OptionalStrategy.Activity);
            UiBootstrap.Instance.Bind(presenter);
            UiBootstrap.Instance.Bind<SimpleTextButtonView.Presenter>();
            UiBootstrap.Instance.Bind<ObservablePresenter<ModelessView>>();

            var view = UiControl.Instance.Create(Locator.viewWithChild);

            Assert.IsFalse(view.View.viewWithModel.gameObject.activeSelf);
        }
        
        [Test]
        public void Set_OptionalObservable_InitialValue_Some_NothingStrategy_ParentUnset()
        {
            var observableText = new OptionalObservable<string>("initial");
            var presenter = new ChildrenViewOptionalPresenter(observableText, ObservableSubscriptionOwnerExtension.OptionalStrategy.Nothing);
            UiBootstrap.Instance.Bind(presenter);
            UiBootstrap.Instance.Bind<SimpleTextButtonView.Presenter>();
            UiBootstrap.Instance.Bind<ObservablePresenter<ModelessView>>();

            var view = UiControl.Instance.Create(Locator.viewWithChild);
            view.Unset();
            observableText.Value = "changed".Some();

            // Unsubscribed, so the value should not change
            Assert.AreEqual("initial", view.View.viewWithModel.Text.text);
            Assert.IsTrue(view.View.viewWithModel.gameObject.activeSelf);
        }
        
        
        [Test]
        public void Set_OptionalObservable_InitialValue_Some_Unsubscribed()
        {
            var observableText = new OptionalObservable<string>("initial");
            var presenter = new ChildrenViewOptionalPresenter(observableText, ObservableSubscriptionOwnerExtension.OptionalStrategy.Nothing);
            UiBootstrap.Instance.Bind(presenter);
            UiBootstrap.Instance.Bind<SimpleTextButtonView.Presenter>();
            UiBootstrap.Instance.Bind<ObservablePresenter<ModelessView>>();

            var view = UiControl.Instance.Create(Locator.viewWithChild);
            presenter.unsubscription.Dispose();
            observableText.Value = "changed".Some();

            // Unsubscribed, so the value should not change
            Assert.AreEqual("initial", view.View.viewWithModel.Text.text);
            Assert.IsTrue(view.View.viewWithModel.gameObject.activeSelf);
        }
        
        [Test]
        public void Set_OptionalObservable_ChangeValue_FromSomeToSome()
        {
            var observableText = new OptionalObservable<string>("initial");
            var presenter = new ChildrenViewOptionalPresenter(observableText, ObservableSubscriptionOwnerExtension.OptionalStrategy.Activity);
            UiBootstrap.Instance.Bind(presenter);
            UiBootstrap.Instance.Bind<SimpleTextButtonView.Presenter>();
            UiBootstrap.Instance.Bind<ObservablePresenter<ModelessView>>();

            var view = UiControl.Instance.Create(Locator.viewWithChild);

            observableText.Value = "changed".Some();
            Assert.AreEqual("changed", view.View.viewWithModel.Text.text);
            Assert.IsTrue(view.View.viewWithModel.gameObject.activeSelf);
        }
        
        [Test]
        public void Set_OptionalObservable_ChangeValue_FromSomeToNone()
        {
            var observableText = new OptionalObservable<string>("initial");
            var presenter = new ChildrenViewOptionalPresenter(observableText, ObservableSubscriptionOwnerExtension.OptionalStrategy.Activity);
            UiBootstrap.Instance.Bind(presenter);
            UiBootstrap.Instance.Bind<SimpleTextButtonView.Presenter>();
            UiBootstrap.Instance.Bind<ObservablePresenter<ModelessView>>();

            var view = UiControl.Instance.Create(Locator.viewWithChild);

            observableText.Value = Option.None<string>();
            Assert.IsFalse(view.View.viewWithModel.gameObject.activeSelf);
        }
        
        [Test]
        public void Set_OptionalObservable_ChangeValue_FromNoneToSome()
        {
            var observableText = new OptionalObservable<string>("initial");
            var presenter = new ChildrenViewOptionalPresenter(observableText, ObservableSubscriptionOwnerExtension.OptionalStrategy.Activity);
            UiBootstrap.Instance.Bind(presenter);
            UiBootstrap.Instance.Bind<SimpleTextButtonView.Presenter>();
            UiBootstrap.Instance.Bind<ObservablePresenter<ModelessView>>();

            var view = UiControl.Instance.Create(Locator.viewWithChild);

            observableText.Value = "changed".Some();
            Assert.AreEqual("changed", view.View.viewWithModel.Text.text);
            Assert.IsTrue(view.View.viewWithModel.gameObject.activeSelf);
        }
        
        [Test]
        public void Set_OptionalObservable_ChangeValue_FromNoneToNone()
        {
            var observableText = new OptionalObservable<string>("initial");
            var presenter = new ChildrenViewOptionalPresenter(observableText, ObservableSubscriptionOwnerExtension.OptionalStrategy.Activity);
            UiBootstrap.Instance.Bind(presenter);
            UiBootstrap.Instance.Bind<SimpleTextButtonView.Presenter>();
            UiBootstrap.Instance.Bind<ObservablePresenter<ModelessView>>();

            var view = UiControl.Instance.Create(Locator.viewWithChild);

            observableText.Value = Option.None<string>();
            Assert.IsFalse(view.View.viewWithModel.gameObject.activeSelf);
        }
        
        [Test]
        public void Set_OptionalObservable_ChangeValue_FromSomeToSome_Unsubscribed()
        {
            var observableText = new OptionalObservable<string>("initial");
            var presenter = new ChildrenViewOptionalPresenter(observableText, ObservableSubscriptionOwnerExtension.OptionalStrategy.Activity);
            UiBootstrap.Instance.Bind(presenter);
            UiBootstrap.Instance.Bind<SimpleTextButtonView.Presenter>();
            UiBootstrap.Instance.Bind<ObservablePresenter<ModelessView>>();

            var view = UiControl.Instance.Create(Locator.viewWithChild);
            observableText.Value = "changed".Some();
            presenter.unsubscription.Dispose();
            observableText.Value = "again".Some();

            // Unsubscribed, so the value should not change
            Assert.AreEqual("changed", view.View.viewWithModel.Text.text);
            Assert.IsTrue(view.View.viewWithModel.gameObject.activeSelf);
        }
        
        [Test]
        public void Set_OptionalObservable_ChangeValue_FromSomeToNone_Unsubscribed()
        {
            var observableText = new OptionalObservable<string>("initial");
            var presenter = new ChildrenViewOptionalPresenter(observableText, ObservableSubscriptionOwnerExtension.OptionalStrategy.Activity);
            UiBootstrap.Instance.Bind(presenter);
            UiBootstrap.Instance.Bind<SimpleTextButtonView.Presenter>();
            UiBootstrap.Instance.Bind<ObservablePresenter<ModelessView>>();

            var view = UiControl.Instance.Create(Locator.viewWithChild);
            observableText.Value = Option.None<string>();
            presenter.unsubscription.Dispose();
            observableText.Value = "again".Some();

            // Unsubscribed, so the value should not change
            Assert.AreNotEqual("again", view.View.viewWithModel.Text.text);
            Assert.IsFalse(view.View.viewWithModel.gameObject.activeSelf);
        }
        
        [Test]
        public void Set_OptionalObservable_ChangeValue_FromSomeToSome_ParentUnset()
        {
            var observableText = new OptionalObservable<string>("initial");
            var presenter = new ChildrenViewOptionalPresenter(observableText, ObservableSubscriptionOwnerExtension.OptionalStrategy.Activity);
            UiBootstrap.Instance.Bind(presenter);
            UiBootstrap.Instance.Bind<SimpleTextButtonView.Presenter>();
            UiBootstrap.Instance.Bind<ObservablePresenter<ModelessView>>();

            var view = UiControl.Instance.Create(Locator.viewWithChild);
            observableText.Value = "changed".Some();
            view.Unset();
            observableText.Value = "again".Some();

            // Unsubscribed, so the value should not change
            Assert.AreEqual("changed", view.View.viewWithModel.Text.text);
            Assert.IsTrue(view.View.viewWithModel.gameObject.activeSelf);
        }
        
        [Test]
        public void Set_OptionalObservable_ChangeValue_FromSomeToNone_ParentUnset()
        {
            var observableText = new OptionalObservable<string>("initial");
            var presenter = new ChildrenViewOptionalPresenter(observableText, ObservableSubscriptionOwnerExtension.OptionalStrategy.Activity);
            UiBootstrap.Instance.Bind(presenter);
            UiBootstrap.Instance.Bind<SimpleTextButtonView.Presenter>();
            UiBootstrap.Instance.Bind<ObservablePresenter<ModelessView>>();

            var view = UiControl.Instance.Create(Locator.viewWithChild);
            observableText.Value = Option.None<string>();
            view.Unset();
            observableText.Value = "again".Some();

            // Unsubscribed, so the value should not change
            Assert.AreNotEqual("again", view.View.viewWithModel.Text.text);
            Assert.IsFalse(view.View.viewWithModel.gameObject.activeSelf);
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

        private class ChildrenViewOptionalPresenter : Presenter<ViewWithChild>
        {
            private readonly IReadOnlyObservable<Option<string>> _observableText;
            private readonly ObservableSubscriptionOwnerExtension.OptionalStrategy _strategy;
            public IDisposable unsubscription;

            public ChildrenViewOptionalPresenter(IReadOnlyObservable<Option<string>> observableText,
                ObservableSubscriptionOwnerExtension.OptionalStrategy strategy)
            {
                _observableText = observableText;
                _strategy = strategy;
            }

            protected override void OnSet(ViewWithChild view, ISubscriptions subs)
            {
                unsubscription = subs.Set(view.viewWithModel, _observableText, _strategy);
            }
        }
    }
}