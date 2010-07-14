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
using Microsoft.Practices.Composite.Regions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StockTraderRI.Infrastructure.Models;
using StockTraderRI.Modules.News.Article;
using StockTraderRI.Modules.News.Tests.Mocks;

namespace StockTraderRI.Modules.News.Tests.Article
{
    /// <summary>
    /// Summary description for NewsReaderPresenterFixture
    /// </summary>
    [TestClass]
    public class NewsReaderPresenterFixture
    {
        [TestMethod]
        public void SetNewsArticlesSetsViewModel()
        {
            var view = new MockNewsReaderView();

            var presenter = new NewsReaderPresenter(view);

            NewsArticle article = new NewsArticle() { Title = "My Title", Body = "My Body" };
            presenter.SetNewsArticle(article);

            Assert.AreSame(article, view.Model);
        }
    }
}