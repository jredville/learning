//===================================================================================
// Microsoft patterns & practices
// Composite Application Guidance for Windows Presentation Foundation and Silverlight
//===================================================================================
// Copyright (c) Microsoft Corporation.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===================================================================================
// The example companies, organizations, products, domain names,
// e-mail addresses, logos, people, places, and events depicted
// herein are fictitious.  No association with any real company,
// organization, product, domain name, email address, logo, person,
// places, or events is intended or should be inferred.
//===================================================================================
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Practices.Composite.Modularity;
using Microsoft.Practices.Composite.Regions;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StockTraderRI.Modules.Watch;
using StockTraderRI.Modules.Watch.AddWatch;
using StockTraderRI.Modules.Watch.Controllers;
using StockTraderRI.Modules.Watch.Services;
using StockTraderRI.Modules.Watch.WatchList;
using StockTraderRI.Modules.WatchList.Tests.Mocks;
using Microsoft.Practices.Composite.TestSupport;

namespace StockTraderRI.Modules.WatchList.Tests
{
    [TestClass]
    public class WatchModuleFixture
    {
        [TestMethod]
        public void RegisterViewsServicesRegistersWatchListView()
        {
            var container = new MockUnityContainer();

            var module = new TestableWatchModule(container, new MockRegionManager());

            module.InvokeRegisterViewsAndServices();

            Assert.AreEqual(typeof(WatchListService), container.Types[typeof(IWatchListService)]);
            Assert.AreEqual(typeof(WatchListView), container.Types[typeof(IWatchListView)]);
            Assert.AreEqual(typeof(WatchListPresentationModel), container.Types[typeof(IWatchListPresentationModel)]);

            Assert.AreEqual(typeof(AddWatchView), container.Types[typeof(IAddWatchView)]);
            Assert.AreEqual(typeof(AddWatchPresenter), container.Types[typeof(IAddWatchPresenter)]);
        }

        [TestMethod]
        public void InitAddsAddWatchControlViewToToolbarRegion()
        {
            var regionManager = new MockRegionManager();
            var container = new MockUnityContainer();
            IModule module = new WatchModule(container, regionManager);
            var contentRegistry = new MockRegionViewRegistry();
            ServiceLocator.SetLocatorProvider(
               () => new MockServiceLocator(
                       () => contentRegistry));

            container.ResolveBag.Add(typeof(IAddWatchPresenter), new MockAddWatchPresenter());
			container.ResolveBag.Add(typeof(IWatchListPresentationModel), new MockWatchListPresentationModel());
			container.ResolveBag.Add(typeof(IWatchListController), new MockWatchListController());
            regionManager.Regions.Add("MainToolBarRegion", new MockRegion());

            Assert.AreEqual(0, contentRegistry.RegisterViewWithRegionRegionNameArg.Count);

            module.Initialize();

            Assert.AreEqual(1, contentRegistry.RegisterViewWithRegionRegionNameArg.Count);
            Assert.IsTrue(contentRegistry.RegisterViewWithRegionRegionNameArg.Contains("MainToolBarRegion"));
        }

        [TestMethod]
        public void ShouldRunControllerOnInitialization()
        {
            var regionManager = new MockRegionManager();
            var container = new MockUnityContainer();
            IModule module = new WatchModule(container, regionManager);
            var contentRegistry = new MockRegionViewRegistry();
            ServiceLocator.SetLocatorProvider(
               () => new MockServiceLocator(
                       () => contentRegistry));
            container.ResolveBag.Add(typeof(IAddWatchPresenter), new MockAddWatchPresenter());
			container.ResolveBag.Add(typeof(IWatchListPresentationModel), new MockWatchListPresentationModel());
            var controller = new MockWatchListController();
			container.ResolveBag.Add(typeof(IWatchListController), controller);
            regionManager.Regions.Add("MainToolBarRegion", new MockRegion());

            module.Initialize();

            Assert.IsTrue(controller.RunCalled);
        }

        internal class TestableWatchModule : WatchModule
        {
            public TestableWatchModule(IUnityContainer container, IRegionManager regionManager)
                : base(container, regionManager)
            {
            }

            public void InvokeRegisterViewsAndServices()
            {
                base.RegisterViewsAndServices();
            }
        }

        class MockAddWatchPresenter : IAddWatchPresenter
        {
            private IAddWatchView _view = new MockAddWatchView();
            public IAddWatchView View
            {
                get { return _view; }
            }
        }

        class MockWatchListPresentationModel : IWatchListPresentationModel
        {
            private IWatchListView _view = new MockWatchListView();

            public ObservableCollection<WatchItem> WatchListItems
            {
                get { throw new System.NotImplementedException(); }
            }

            public IWatchListView View
            {
                get { return _view; }
            }
        }
    }

    internal class MockWatchListController : IWatchListController
    {
        public bool RunCalled;

        public void Run()
        {
            RunCalled = true;
        }
    }

    internal class MockServiceLocator : ServiceLocatorImplBase
    {
        public Func<object> ResolveMethod;

        public MockServiceLocator(Func<object> resolveMethod)
        {
            ResolveMethod = resolveMethod;
        }

        protected override object DoGetInstance(Type serviceType, string key)
        {
            return ResolveMethod();
        }

        protected override IEnumerable<object> DoGetAllInstances(Type serviceType)
        {
            return null;
        }
    }

    internal class MockRegionViewRegistry : IRegionViewRegistry
    {
        public List<string> RegisterViewWithRegionRegionNameArg = new List<string>();

        public event EventHandler<ViewRegisteredEventArgs> ContentRegistered;

        public IEnumerable<object> GetContents(string regionName)
        {
            throw new System.NotImplementedException();
        }

        public void RegisterViewWithRegion(string regionName, Type viewType)
        {
            throw new System.NotImplementedException();
        }

        public void RegisterViewWithRegion(string regionName, Func<object> getContentDelegate)
        {
            RegisterViewWithRegionRegionNameArg.Add(regionName);
        }
    }
}
