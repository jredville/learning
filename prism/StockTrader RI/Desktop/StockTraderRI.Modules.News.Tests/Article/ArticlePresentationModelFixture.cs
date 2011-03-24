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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StockTraderRI.Infrastructure.Interfaces;
using StockTraderRI.Infrastructure.Models;
using StockTraderRI.Modules.News.Article;
using StockTraderRI.Modules.News.Tests.Mocks;

namespace StockTraderRI.Modules.News.Tests.Article
{
    [TestClass]
    public class ArticlePresentationModelFixture
    {
        [TestMethod]
        public void CanInitPresenter()
        {
            MockArticleView view = new MockArticleView();
            MockNewsFeedService newsFeedService = new MockNewsFeedService();

            ArticlePresentationModel presentationModel = new ArticlePresentationModel(view, newsFeedService);
            Assert.AreEqual<IArticleView>(view, presentationModel.View);
        }


        [TestMethod]
        public void ShowNewsDoesNothingIfNewsFeedHasNoNews()
        {
            MockArticleView view = new MockArticleView();
            MockRegionManager regionManager = new MockRegionManager();
            regionManager.Regions.Add("NewsRegion", new MockNewsRegion());
            MockNewsFeedService newsFeedService = new MockNewsFeedService();
            newsFeedService.NewsArticles = null;
            ArticlePresentationModel presentationModel = new ArticlePresentationModel(view, newsFeedService);
            presentationModel.Controller = new MockNewsController();

            presentationModel.SetTickerSymbol("InexistentNews");

            Assert.AreEqual(0, ((MockNewsRegion)regionManager.Regions["NewsRegion"]).AddedViews.Count);
        }

        [TestMethod]
        public void ShowNewsPassesNewsContentToView()
        {
            MockArticleView view = new MockArticleView();
            MockNewsFeedService newsFeedService = new MockNewsFeedService();
            newsFeedService.NewsArticles = new List<NewsArticle>() { new NewsArticle() { Title = "FUND0", Body = "My custom body text" } };
            ArticlePresentationModel presentationModel = new ArticlePresentationModel(view, newsFeedService);
            presentationModel.Controller = new MockNewsController();

            presentationModel.SetTickerSymbol("FUND0");

            Assert.AreEqual(1, view.Model.Articles.Count);
            Assert.AreEqual("My custom body text", view.Model.Articles[0].Body);
        }

        [TestMethod]
        public void ViewContainsCorrectModelHeaderInfoAfterSetTickerSymbol()
        {
            var view = new MockArticleView();
            var newsFeedService = new MockNewsFeedService();
            newsFeedService.NewsArticles = new List<NewsArticle>() { new NewsArticle() { Title = "MySymbol", IconUri = "MyPath" } };
            var presenter = new ArticlePresentationModel(view, newsFeedService);
            presenter.Controller = new MockNewsController();

            presenter.SetTickerSymbol("MyTitle");

            Assert.IsNotNull(view.Model);
            Assert.IsNotNull(view.Model.Articles);
            Assert.AreEqual(1, view.Model.Articles.Count);
            Assert.AreEqual("MyPath", view.Model.Articles[0].IconUri);
            Assert.AreEqual("MySymbol", view.Model.Articles[0].Title);
        }

        [TestMethod]
        public void ArticlePresenterNotifiesControllerOnItemChange()
        {
            var view = new MockArticleView();
            var newsFeedService = new MockNewsFeedService();
            var mockController = new MockNewsController();
            newsFeedService.NewsArticles = new List<NewsArticle>() { new NewsArticle() { Title = "MySymbol", IconUri = "MyPath" },
                                                                     new NewsArticle() { Title = "OtherSymbol", IconUri = "OtherPath" }};
            var presenter = new ArticlePresentationModel(view, newsFeedService);
            presenter.Controller = mockController;
            presenter.SetTickerSymbol("DoesNotMatter");

            presenter.SelectedArticle = newsFeedService.NewsArticles[1];

            Assert.IsTrue(mockController.CurrentNewsArticleChangedCalled);
        }

        [TestMethod]
        public void ArticlePresenterCallControllerToShowNewsReader()
        {
            var view = new MockArticleView();
            var newsFeedService = new MockNewsFeedService();
            var mockController = new MockNewsController();

            var presenter = new ArticlePresentationModel(view, newsFeedService);
            presenter.Controller = mockController;

            view.RaiseShowNewsReaderEvent();

            Assert.IsTrue(mockController.ShowNewsReaderCalled);

        }



        private class MockNewsFeedService : INewsFeedService
        {
            public IList<NewsArticle> NewsArticles = new List<NewsArticle>()
                                                         {
                                                                 new NewsArticle()
                                                                     {Title = "Title", IconUri = "IconUri", Body = "Body", PublishedDate = DateTime.Now}
                                                         };


            #region INewsFeedService Members

            public IList<NewsArticle> GetNews(string tickerSymbol)
            {
                return this.NewsArticles;
            }

            public bool HasNews(string tickerSymbol)
            {
                throw new NotImplementedException();
            }

            public event EventHandler<NewsFeedEventArgs> Updated = delegate { };

            #endregion
        }
    }
}