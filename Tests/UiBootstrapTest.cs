using System;
using NUnit.Framework;
using UnityEngine;
using Yaga;
using Yaga.Exceptions;
using Yaga.Test;

namespace Tests
{
    public class UiBootstrapTest : BaseUiTest
    {
        [SetUp]
        public void SetUp()
        {
            UiBootstrap.InitializeSingleton();
            UiControl.InitializeSingleton(Locator.canvasPrefab);
        }

        [Test]
        public void Bind_PresenterIsNull_ThrowsException()
        {
            Assert.Catch<ArgumentNullException>(() => UiBootstrap.Bind(null));
        }

        [Test]
        public void Bind_PresenterWithoutDefaultConstructor_ThrowsException()
        {
            Assert.Catch<NoDefaultConstructorForPresenterException>(() => UiBootstrap.Bind<PresenterWithConstructor>());
        }

        [Test]
        public void Bind_PresenterWithoutView_ThrowsException()
        {
            Assert.Catch<PresenterBindingException>(UiBootstrap.Bind<PresenterWithoutView>);
        }
        
        [Test]
        public void Bind_MultiplePresenters_ThrowsException()
        {
            UiBootstrap.Bind<PresenterA>();
            Assert.Catch<MultiplePresenterException>(UiBootstrap.Bind<PresenterB>);
        }

        [Test]
        public void Set_ViewIsNull_ThrowsException()
        {
            Assert.Catch<ArgumentNullException>(() =>
                UiBootstrap.Instance.Set<ModelessView, Unit>(null, Unit.Instance));
        }

        [Test]
        public void Set_ModelIsNull_ThrowsException()
        {
            Assert.Catch<ArgumentNullException>(() =>
                UiBootstrap.Instance.Set(Locator.simpleTextButtonView, default(string)));
        }

        [Test]
        public void Set_NoPresenter_ThrowsException()
        {
            var view = GameObject.Instantiate(Locator.modelessView);
            Assert.Catch<PresenterNotFoundException>(() => UiBootstrap.Instance.Set(view, Unit.Instance));
        }

        [Test]
        public void ClearPresenters_NoPresentersAfter()
        {
            var view = GameObject.Instantiate(Locator.modelessView);
            UiBootstrap.Bind<PresenterA>();
            UiBootstrap.Instance.Set(view, Unit.Instance);
            UiBootstrap.ClearPresenters();
            Assert.Catch<PresenterNotFoundException>(() => UiBootstrap.Instance.Set(view, Unit.Instance));
        }

        private class PresenterA : Presenter<ModelessView>
        {
        }

        private class PresenterB : Presenter<ModelessView>
        {
        }

        private class PresenterWithoutView : IPresenter
        {
            public bool AcceptableView(Type viewType) => true;
        }

        private class PresenterWithConstructor : Presenter<ModelessView>
        {
            public PresenterWithConstructor(int value)
            {
            }
        }
    }
}