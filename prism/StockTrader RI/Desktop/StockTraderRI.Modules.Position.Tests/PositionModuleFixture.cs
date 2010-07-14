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
using System.Windows.Input;
using Microsoft.Practices.Composite.Regions;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StockTraderRI.Infrastructure.Interfaces;
using StockTraderRI.Modules.Position.Controllers;
using StockTraderRI.Modules.Position.Interfaces;
using StockTraderRI.Modules.Position.Orders;
using StockTraderRI.Modules.Position.PositionSummary;
using StockTraderRI.Modules.Position.Services;
using StockTraderRI.Modules.Position.Tests.Mocks;
using Microsoft.Practices.Composite.TestSupport;

namespace StockTraderRI.Modules.Position.Tests
{
    [TestClass]
    public class PositionModuleFixture
    {
        [TestMethod]
        [DeploymentItem("Data", "Data")]
        public void RegisterViewsServicesRegistersViewsAndServices()
        {
            try
            {
                MockUnityContainer container = new MockUnityContainer();

                var contentRegistry = new MockRegionViewRegistry();
				container.ResolveBag.Add(typeof(IRegionViewRegistry), contentRegistry);
                ServiceLocator.SetLocatorProvider(
                    () => new MockServiceLocator(
                            () => contentRegistry));

                TestablePositionModule module = new TestablePositionModule(container, new MockRegionManager());

                module.InvokeRegisterViewsAndServices();

                Assert.AreEqual(typeof(AccountPositionService), container.Types[typeof(IAccountPositionService)]);
                Assert.AreEqual(typeof(PositionSummaryView), container.Types[typeof(IPositionSummaryView)]);
                Assert.AreEqual(typeof(PositionSummaryPresentationModel), container.Types[typeof(IPositionSummaryPresentationModel)]);
                Assert.AreEqual(typeof(OrdersView), container.Types[typeof(IOrdersView)]);
                Assert.AreEqual(typeof(OrdersPresentationModel), container.Types[typeof(IOrdersPresentationModel)]);
                Assert.AreEqual(typeof(OrderDetailsView), container.Types[typeof(IOrderDetailsView)]);
                Assert.AreEqual(typeof(OrderDetailsPresentationModel), container.Types[typeof(IOrderDetailsPresentationModel)]);
                Assert.AreEqual(typeof(OrderCommandsView), container.Types[typeof(IOrderCommandsView)]);
                Assert.AreEqual(typeof(OrderCompositeView), container.Types[typeof(IOrderCompositeView)]);
                Assert.AreEqual(typeof(OrderCompositePresentationModel), container.Types[typeof(IOrderCompositePresentationModel)]);
                Assert.AreEqual(typeof(OrdersController), container.Types[typeof(IOrdersController)]);
                Assert.AreEqual(typeof(XmlOrdersService), container.Types[typeof(IOrdersService)]);
            }
            finally
            {
                ServiceLocator.SetLocatorProvider(null);
            }
        }

        [TestMethod]
        public void InitRegistersOrdersToolbarViewToToolbarRegion()
        {
            try
            {
                MockRegion toolbarRegion = new MockRegion();
                MockRegion mainRegion = new MockRegion();
                MockRegionManager regionManager = new MockRegionManager();
                var container = new MockUnityContainer();
                container.ResolveBag.Add(typeof(IOrdersController), new MockOrdersController());
				container.ResolveBag.Add(typeof(IPositionSummaryPresentationModel), new MockPositionSummaryPresenter());

                var contentRegistry = new MockRegionViewRegistry();
                ServiceLocator.SetLocatorProvider(
                   () => new MockServiceLocator(
                           () => contentRegistry));

                PositionModule module = new PositionModule(container, regionManager);
                regionManager.Regions.Add("MainRegion", mainRegion);
                regionManager.Regions.Add("CollapsibleRegion", new MockRegion());

                Assert.AreEqual(0, contentRegistry.RegisterViewWithRegionRegionNameArg.Count);

                module.Initialize();

                Assert.AreEqual(2, contentRegistry.RegisterViewWithRegionRegionNameArg.Count);
                Assert.IsTrue(contentRegistry.RegisterViewWithRegionRegionNameArg.Contains("ResearchRegion"));
                Assert.IsTrue(contentRegistry.RegisterViewWithRegionRegionNameArg.Contains("MainRegion"));
            }
            finally
            {
                ServiceLocator.SetLocatorProvider(null);
            }
        }

        internal class TestablePositionModule : PositionModule
        {
            public TestablePositionModule(IUnityContainer container, IRegionManager regionManager)
                : base(container, regionManager)
            {
            }

            public void InvokeRegisterViewsAndServices()
            {
                base.RegisterViewsAndServices();
            }
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

    internal class MockPositionSummaryPresenter : IPositionSummaryPresentationModel
    {
        #region IPositionSummaryPresenter Members

        private IPositionSummaryView _view = new MockPositionSummaryView();

        public IPositionSummaryView View
        {
            get { return _view; }
            set { _view = value; }
        }


        public IObservablePosition Position
        {
            get { throw new System.NotImplementedException(); }
        }


        public ICommand BuyCommand
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
                throw new System.NotImplementedException();
            }
        }

        public ICommand SellCommand
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
                throw new System.NotImplementedException();
            }
        }

        #endregion

        #region IPositionSummaryPresentationModel Members


        public string HeaderInfo
        {
            get { throw new System.NotImplementedException(); }
        }

        #endregion
    }
}
