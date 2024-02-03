using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using Yaga;
using Yaga.Exceptions;
using Yaga.Reactive;
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
        public void CheckModelessNullSetView()
        {
            Assert.Catch<ArgumentNullException>(() => UiBootstrap.Instance.Set<ModelessView, Unit>(null, Unit.Instance));
        }

        [Test]
        public void MultiplePresentersTest()
        {
            var view = GameObject.Instantiate(Locator.modelessView);
            UiBootstrap.Bind<PresenterA>();
            UiBootstrap.Bind<PresenterB>();
            Assert.Catch<MultiplePresenterException>(() => UiBootstrap.Instance.Set(view, Unit.Instance));
        }

        [Test]
        public void PresenterNotFoundTest()
        {
            var view = GameObject.Instantiate(Locator.modelessView);
            Assert.Catch<PresenterNotFoundException>(() => UiBootstrap.Instance.Set(view, Unit.Instance));
        }

        [Test]
        public void ClearPresentersTest()
        {
            var view = GameObject.Instantiate(Locator.modelessView);
            UiBootstrap.Bind<PresenterA>();
            UiBootstrap.Instance.Set(view, Unit.Instance);
            UiBootstrap.ClearPresenters();
            Assert.Catch<PresenterNotFoundException>(() => UiBootstrap.Instance.Set(view, Unit.Instance));
        }

        [Test]
        public void NoDefaultConstructorParameterTest()
        {
            Assert.Catch<NoDefaultConstructorForPresenterException>(() => UiBootstrap.Bind<PresenterWithConstructor>());
        }

        [Test]
        public void BootstrapConstructorTest()
        {
            var view = GameObject.Instantiate(Locator.modelessView);
            var bootstrap = new UiBootstrap(new List<IPresenter> { new PresenterA() });
            Assert.Catch<PresenterNotFoundException>(() => UiBootstrap.Instance.Set(view, Unit.Instance));
            UiBootstrap.InitializeSingleton(bootstrap);
            UiBootstrap.Instance.Set(view, Unit.Instance);
        }

        [Test]
        public void ExceptionOnPresenterWithoutView()
        {
            Assert.Catch<PresenterBindingException>(UiBootstrap.Bind<PresenterWithoutView>);
        }

        [Test]
        public void BootstrapConstructorNullTest()
        {
            Assert.Catch<ArgumentNullException>(() => new UiBootstrap(null));
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