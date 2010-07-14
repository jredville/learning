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
using Microsoft.Practices.Composite.Regions;
using Microsoft.Practices.ServiceLocation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StockTraderRI.Modules.Watch;
using StockTraderRI.Modules.Watch.Controllers;
using StockTraderRI.Modules.Watch.WatchList;
using StockTraderRI.Modules.WatchList.Tests.Mocks;

namespace StockTraderRI.Modules.WatchList.Tests.Controllers
{
    [TestClass]
    public class WatchListControllerFixture
    {
        [TestMethod]
        public void ShouldRegisterWatchListViewOnRegion()
        {
            try
            {
                var mockRegionContentRegistry = new MockRegionViewRegistry();
                ServiceLocator.SetLocatorProvider(
                    () => new MockServiceLocator(
                        () => mockRegionContentRegistry));
                var presentationModel = new MockWatchListPresentationModel();
                var watchListView = new MockWatchListView();
                presentationModel.View = watchListView;
                WatchListController controller = 
                    new WatchListController ( new MockRegionManager(), presentationModel);

                controller.Run();

                Assert.AreEqual(1, mockRegionContentRegistry.RegisteredViews.Count);
                Assert.IsTrue(mockRegionContentRegistry.RegisteredViews.ContainsKey("MainRegion"));
                Assert.AreSame(watchListView, mockRegionContentRegistry.RegisteredViews["MainRegion"]);
            }
            finally
            {
                ServiceLocator.SetLocatorProvider(() => null);
            }
        }

        [TestMethod]
        public void ShouldActivateViewOnWatchItemAdded()
        {
            try
            {
                var mockRegionContentRegistry = new MockRegionViewRegistry();
                ServiceLocator.SetLocatorProvider(
                    () => new MockServiceLocator(
                              () => mockRegionContentRegistry));
                var watchListView = new MockWatchListView();
                var regionManager = new MockRegionManager();
                var region = new MockRegion();
                region.Add(watchListView);
                regionManager.Regions.Add("MainRegion", region);
                var presentationModel = new MockWatchListPresentationModel { View = watchListView };
                WatchListController controller =
                    new WatchListController(regionManager, presentationModel);

                controller.Run();

                presentationModel.WatchListItems.Add(new WatchItem("MySymbol", 10m));

                Assert.IsTrue(region.ActivateCalled);
                Assert.AreSame(watchListView, region.ActivateArg);
            }
            finally
            {
                ServiceLocator.SetLocatorProvider(() => null);
            }
        }

        private class MockWatchListPresentationModel : IWatchListPresentationModel
        {
            public MockWatchListPresentationModel()
            {
                WatchListItems = new ObservableCollection<WatchItem>();
            }

            public ObservableCollection<WatchItem> WatchListItems { get; set; }

            public IWatchListView View { get; set; }
        }

        private class MockRegionViewRegistry : IRegionViewRegistry
        {
            public event EventHandler<ViewRegisteredEventArgs> ContentRegistered;

            public Dictionary<string, object> RegisteredViews = new Dictionary<string, object>();

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
                RegisteredViews[regionName] = getContentDelegate();
            }
        }
    }
}
