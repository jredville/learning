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
using Microsoft.Practices.Composite.Events;
using Microsoft.Practices.Composite.Presentation.Events;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StockTraderRI.Infrastructure;
using StockTraderRI.Infrastructure.Models;
using StockTraderRI.Modules.News.Article;
using StockTraderRI.Modules.News.Controllers;
using StockTraderRI.Modules.News.Tests.Mocks;
using Microsoft.Practices.Composite.Regions;

namespace StockTraderRI.Modules.News.Tests.Controllers
{
    [TestClass]
    public class NewsControllerFixture
    {
        [TestMethod]
        public void ShowNewsResolvesPresenterAndCallsSetTickerSymbolOnItAndAddsNamedViewToRegion()
        {
            var presenter = new MockArticlePresentationModel();
            var eventAggregator = new MockEventAggregator();
            eventAggregator.AddMapping<TickerSymbolSelectedEvent>(new MockTickerSymbolSelectedEvent());
            var controller = new NewsController(CreateRegionManager(), presenter, eventAggregator, new MockNewsReaderPresenter());

            controller.ShowNews("Test");

            Assert.IsNotNull(presenter.SetTickerSymbolArgumentCompanySymbol);
            Assert.AreEqual("Test", presenter.SetTickerSymbolArgumentCompanySymbol);
        }

        [TestMethod]
        public void ControllerShowNewsWhenRasingGlobalEvent()
        {
            var presenter = new MockArticlePresentationModel();
            var eventAggregator = new MockEventAggregator();
            var tickerSymbolSelectedEvent = new MockTickerSymbolSelectedEvent();
            eventAggregator.AddMapping<TickerSymbolSelectedEvent>(tickerSymbolSelectedEvent);
            var controller = new NewsController(CreateRegionManager(), presenter, eventAggregator, new MockNewsReaderPresenter());

            controller.Run();

            Assert.IsNotNull(tickerSymbolSelectedEvent.SubscribeArgumentAction);

            tickerSymbolSelectedEvent.SubscribeArgumentAction("TEST_SYMBOL");
            Assert.AreEqual("TEST_SYMBOL", presenter.SetTickerSymbolArgumentCompanySymbol);
        }

        [TestMethod]
        public void ShouldNotifyReaderWhenCurrentNewsArticleChanges()
        {
            var presenter = new MockArticlePresentationModel();
            var eventAggregator = new MockEventAggregator();
            eventAggregator.AddMapping<TickerSymbolSelectedEvent>(new MockTickerSymbolSelectedEvent());
            var newsReaderPresenter = new MockNewsReaderPresenter();
            var controller = new NewsController(CreateRegionManager(), presenter, eventAggregator, newsReaderPresenter);

            controller.CurrentNewsArticleChanged(new NewsArticle() { Title = "SomeTitle", Body = "Newsbody" });

            Assert.IsTrue(newsReaderPresenter.SetNewsArticleCalled);
        }

        [TestMethod]
        public void ControllerShowNewsViewWhenArticlePresenterReceivesEvent()
        {
            var articlePresentationModel = new MockArticlePresentationModel();
            var eventAggregator = new MockEventAggregator();
            eventAggregator.AddMapping<TickerSymbolSelectedEvent>(new MockTickerSymbolSelectedEvent());
            var newsReaderPresenter = new MockNewsReaderPresenter();
            var regionManager = CreateRegionManager();
            var shellRegion = (MockRegion)regionManager.Regions["SecondaryRegion"];

            var controller = new NewsController(regionManager, articlePresentationModel, eventAggregator, newsReaderPresenter);

            controller.ShowNewsReader();

            Assert.IsTrue(shellRegion.ActivateCalled);
            Assert.AreEqual(newsReaderPresenter.View, shellRegion.ActivateArg);
        }

        private MockRegionManager CreateRegionManager()
        {
            var regionManager = new MockRegionManager();
            regionManager.Regions.Add("SecondaryRegion", new MockRegion());
            regionManager.Regions.Add("NewsRegion", new MockRegion());
            regionManager.Regions.Add("ResearchRegion", new MockRegion());

            return regionManager;
        }

        internal class MockArticlePresentationModel : IArticlePresentationModel
        {
            public MockArticleView MockArticleView = new MockArticleView();
            public string SetTickerSymbolArgumentCompanySymbol;
            public void SetTickerSymbol(string companySymbol)
            {
                SetTickerSymbolArgumentCompanySymbol = companySymbol;
            }

            public IArticleView View
            {
                get { return MockArticleView; }
            }

            public INewsController Controller { get; set; }
        }

        internal class MockTickerSymbolSelectedEvent : TickerSymbolSelectedEvent
        {
            public Action<string> SubscribeArgumentAction;
            public Predicate<string> SubscribeArgumentFilter;
            public override SubscriptionToken Subscribe(Action<string> action, ThreadOption threadOption, bool keepSubscriberReferenceAlive, Predicate<string> filter)
            {
                SubscribeArgumentAction = action;
                SubscribeArgumentFilter = filter;
                return null;
            }
        }
    }

    internal class MockNewsReaderPresenter : INewsReaderPresenter
    {
        public bool SetNewsArticleCalled { get; set; }

        public INewsReaderView View { get; set; }

        public MockNewsReaderPresenter()
        {
            View = new MockNewsReaderView();
        }

        public void SetNewsArticle(NewsArticle article)
        {
            SetNewsArticleCalled = true;
        }
    }
}
