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
using Microsoft.Practices.Composite.Regions;
using Microsoft.Practices.ServiceLocation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StockTraderRI.Infrastructure.Interfaces;
using StockTraderRI.Modules.Market.Tests.Mocks;
using StockTraderRI.Modules.Market.TrendLine;
using Microsoft.Practices.Composite.TestSupport;

namespace StockTraderRI.Modules.Market.Tests
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class MarketModuleFixture
    {
        [TestMethod]
        [DeploymentItem("Data/MarketHistory.xml", "Data")]
        public void CanInitModule()
        {
            try
            {
                MockUnityContainer container = new MockUnityContainer();
                var contentRegistry = new MockRegionViewRegistry();
                container.ResolveBag.Add(typeof(IRegionViewRegistry), contentRegistry);
                ServiceLocator.SetLocatorProvider(
                    () => new MockServiceLocator(
                            () => contentRegistry));
                MarketModule module = new MarketModule(container, new MockRegionManager());

                module.Initialize();

                Assert.IsTrue(container.Types.ContainsKey(typeof(IMarketHistoryService)));
                Assert.IsTrue(container.Types.ContainsKey(typeof(IMarketFeedService)));
                Assert.IsTrue(container.Types.ContainsKey(typeof(ITrendLineView)));
                Assert.IsTrue(container.Types.ContainsKey(typeof(ITrendLinePresentationModel)));
            }
            finally 
            {
                ServiceLocator.SetLocatorProvider(() => null);
            }
        }
    }

    internal class MockRegionViewRegistry : IRegionViewRegistry
    {
        public void RegisterViewWithRegion(string regionName, Func<object> getContentDelegate)
        {
        }

        #region Not Implemented members

        public event EventHandler<ViewRegisteredEventArgs> ContentRegistered;

        public IEnumerable<object> GetContents(string regionName)
        {
            throw new System.NotImplementedException();
        }

        public void RegisterViewWithRegion(string regionName, Type viewType)
        {
            throw new System.NotImplementedException();
        }

        #endregion
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

    internal class MockRegionManager : IRegionManager
    {
        #region Not Implemented members

        public IRegionCollection Regions
        {
            get { throw new System.NotImplementedException(); }
        }

        public IRegionManager CreateRegionManager()
        {
            throw new System.NotImplementedException();
        }

        #endregion
    }
}
