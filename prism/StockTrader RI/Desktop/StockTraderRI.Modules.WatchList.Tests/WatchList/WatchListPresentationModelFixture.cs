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
using System.Linq;
using Microsoft.Practices.Composite.Events;
using Microsoft.Practices.Composite.Presentation.Events;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StockTraderRI.Infrastructure;
using StockTraderRI.Modules.Watch;
using StockTraderRI.Modules.Watch.WatchList;
using StockTraderRI.Modules.WatchList.Tests.Mocks;

namespace StockTraderRI.Modules.WatchList.Tests.WatchList
{
    [TestClass]
    public class WatchListPresentationModelFixture
    {
        MockWatchListView view;
        MockWatchListService watchListService;
        MockMarketFeedService marketFeedService;
        MockEventAggregator eventAggregator;

        [TestInitialize]
        public void SetUp()
        {
            view = new MockWatchListView();
            watchListService = new MockWatchListService();
            marketFeedService = new MockMarketFeedService();
            eventAggregator = new MockEventAggregator();
        }

        [TestMethod]
        public void CanInitPresenter()
        {
            eventAggregator.AddMapping<MarketPricesUpdatedEvent>(new MockMarketPricesUpdatedEvent());
            WatchListPresentationModel presentationModel = CreatePresenter();

            Assert.AreEqual<IWatchListView>(view, presentationModel.View);
        }

        [TestMethod]
        public void ClickingOnTheRemoveMenuItemCallsTheRemoveSelectedItemMethod()
        {
            eventAggregator.AddMapping<MarketPricesUpdatedEvent>(new MockMarketPricesUpdatedEvent());

            watchListService.MockWatchList.Add("TEST");
            WatchListPresentationModel presentationModel = CreatePresenter();

            Assert.AreEqual(1, watchListService.MockWatchList.Count);

            presentationModel.RemoveWatchCommand.Execute("TEST");

            Assert.AreEqual(0, watchListService.MockWatchList.Count);
        }

        [TestMethod]
        public void CanGetItemsFromWatchListServiceAndPutInView()
        {
            eventAggregator.AddMapping<MarketPricesUpdatedEvent>(new MockMarketPricesUpdatedEvent());

            watchListService.MockWatchList.Add("TESTFUND0");
            watchListService.MockWatchList.Add("TESTFUND1");

            WatchListPresentationModel presentationModel = CreatePresenter();

            Assert.IsNotNull(view.Model);
            Assert.IsNotNull(view.Model.WatchListItems);
            Assert.AreEqual(2, view.Model.WatchListItems.Count);
            Assert.AreEqual<string>("TESTFUND0", view.Model.WatchListItems[0].TickerSymbol);
            Assert.AreEqual<string>("TESTFUND1", view.Model.WatchListItems[1].TickerSymbol);
        }

        [TestMethod]
        public void PresenterObservesWatchListAndUpdatesView()
        {
            eventAggregator.AddMapping<MarketPricesUpdatedEvent>(new MockMarketPricesUpdatedEvent());

            watchListService.MockWatchList.Add("TESTFUND0");

            WatchListPresentationModel presentationModel = CreatePresenter();

            Assert.AreEqual(1, view.Model.WatchListItems.Count());

            watchListService.MockWatchList.Add("TESTFUND1");

            Assert.AreEqual(2, view.Model.WatchListItems.Count());
            Assert.AreEqual<string>("TESTFUND1", view.Model.WatchListItems[1].TickerSymbol);
        }


        [TestMethod]
        public void ViewGetsCurrentPriceForSymbolRetrievedFromMarketFeedService()
        {
            eventAggregator.AddMapping<MarketPricesUpdatedEvent>(new MockMarketPricesUpdatedEvent());

            watchListService.MockWatchList.Add("TESTFUND0");
            marketFeedService.SetPrice("TESTFUND0", 15.5m);

            WatchListPresentationModel presentationModel = CreatePresenter();

            WatchItem watchItem = view.Model.WatchListItems[0];
            Assert.AreEqual<string>("TESTFUND0", watchItem.TickerSymbol);
            Assert.IsNotNull(watchItem.CurrentPrice);
            Assert.AreEqual<decimal>(15.5m, watchItem.CurrentPrice.Value);
        }

        [TestMethod]
        public void IfPriceIsNotAvailableForSymbolSetsNullCurrentPriceInView()
        {
            eventAggregator.AddMapping<MarketPricesUpdatedEvent>(new MockMarketPricesUpdatedEvent());

            watchListService.MockWatchList.Add("NONEXISTING");
            marketFeedService.MockSymbolExists = false;

            WatchListPresentationModel presentationModel = CreatePresenter();

            WatchItem watchItem = view.Model.WatchListItems[0];
            Assert.AreEqual<string>("NONEXISTING", watchItem.TickerSymbol);
            Assert.IsNull(watchItem.CurrentPrice);
        }

        [TestMethod]
        public void PresenterObservesMarketFeedAndUpdatesView()
        {
            var marketPricesUpdatedEvent = new MockMarketPricesUpdatedEvent();
            eventAggregator.AddMapping<MarketPricesUpdatedEvent>(marketPricesUpdatedEvent);

            marketFeedService.feedData.Add("TESTFUND0", 15.5m);
            watchListService.MockWatchList.Add("TESTFUND0");

            WatchListPresentationModel presentationModel = CreatePresenter();

            Assert.AreEqual<decimal>(15.5m, view.Model.WatchListItems[0].CurrentPrice.Value);

            Assert.IsNotNull(marketPricesUpdatedEvent.SubscribeArgumentAction);
            Assert.AreEqual(ThreadOption.UIThread, marketPricesUpdatedEvent.SubscribeArgumentThreadOption);

            marketPricesUpdatedEvent.SubscribeArgumentAction(new Dictionary<string, decimal> { { "TESTFUND0", 25.3m } });

            Assert.AreEqual<decimal>(25.3m, view.Model.WatchListItems[0].CurrentPrice.Value);
        }

        [TestMethod]
        public void PresentationModelShouldHaveHeaderInfoSet()
        {
            eventAggregator.AddMapping<MarketPricesUpdatedEvent>(new MockMarketPricesUpdatedEvent());

            WatchListPresentationModel presentationModel = CreatePresenter();
            Assert.AreEqual("WATCH LIST", view.Model.HeaderInfo);
        }

        [TestMethod]
        public void ShouldPublishTickerSymbolSelectedOnSelectionChanged()
        {
            var tickerSymbolSelectedEvent = new MockTickerSymbolSelectedEvent();
            eventAggregator.AddMapping<TickerSymbolSelectedEvent>(tickerSymbolSelectedEvent);
            eventAggregator.AddMapping<MarketPricesUpdatedEvent>(new MockMarketPricesUpdatedEvent());

            WatchListPresentationModel presentationModel = CreatePresenter();

            presentationModel.CurrentWatchItem = new WatchItem("MyTickerSymbol", 10m);

            Assert.IsTrue(tickerSymbolSelectedEvent.PublishCalled);
            Assert.AreEqual("MyTickerSymbol", tickerSymbolSelectedEvent.PublishArg);
        }

        private WatchListPresentationModel CreatePresenter()
        {
            return new WatchListPresentationModel(view, watchListService, marketFeedService, eventAggregator);
        }
    }

    internal class MockTickerSymbolSelectedEvent : TickerSymbolSelectedEvent
    {
        public bool PublishCalled;
        public string PublishArg;

        public override void Publish(string payload)
        {
            PublishCalled = true;
            PublishArg = payload;
        }
    }

    internal class MockMarketPricesUpdatedEvent : MarketPricesUpdatedEvent
    {
        public Action<IDictionary<string, decimal>> SubscribeArgumentAction;
        public Predicate<IDictionary<string, decimal>> SubscribeArgumentFilter;
        public ThreadOption SubscribeArgumentThreadOption;

        public override SubscriptionToken Subscribe(Action<IDictionary<string, decimal>> action, ThreadOption threadOption, bool keepSubscriberReferenceAlive, Predicate<IDictionary<string, decimal>> filter)
        {
            SubscribeArgumentAction = action;
            SubscribeArgumentFilter = filter;
            SubscribeArgumentThreadOption = threadOption;
            return null;
        }
    }
}