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
using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StockTraderRI.Infrastructure.Interfaces;
using StockTraderRI.Modules.News.Article;
using StockTraderRI.Modules.News.Controllers;
using StockTraderRI.Modules.News.Services;
using StockTraderRI.Modules.News.Tests.Mocks;
using Microsoft.Practices.Composite.TestSupport;

namespace StockTraderRI.Modules.News.Tests
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class NewsModuleFixture
    {
        private MockUnityContainer container;
        private MockNewsController controller;

        [TestMethod]
        public void NewsModuleRegistersNewsViewAndNewsFeedService()
        {
            TestableNewsModule newsModule = CreateTestableNewsModule();

            newsModule.InvokeRegisterViewsAndServices();

            Assert.AreEqual(typeof(ArticleView), container.Types[typeof(IArticleView)]);
            Assert.AreEqual(typeof(NewsController), container.Types[typeof(INewsController)]);
            Assert.AreEqual(typeof(ArticlePresentationModel), container.Types[typeof(IArticlePresentationModel)]);
            Assert.AreEqual(typeof(NewsFeedService), container.Types[typeof(INewsFeedService)]);
#if !SILVERLIGHT
            Assert.AreEqual(typeof(NewsReaderPresenter), container.Types[typeof(INewsReaderPresenter)]);
            Assert.AreEqual(typeof(NewsReader), container.Types[typeof(INewsReaderView)]);
#endif
        }

        [TestMethod]
        public void InitCallsRunOnNewsController()
        {
            var newsModule = CreateTestableNewsModule();

            newsModule.Initialize();

            Assert.IsTrue(controller.RunCalled);
        }

        private TestableNewsModule CreateTestableNewsModule()
        {
            this.container = new MockUnityContainer();
            this.controller = new MockNewsController();

            container.ResolveBag.Add(typeof(INewsController), controller);
			container.ResolveBag.Add(typeof(INewsReaderView), null);

            return new TestableNewsModule(container);
        }

        internal class TestableNewsModule : NewsModule
        {
            public TestableNewsModule(IUnityContainer container)
                : base(container)
            {
            }

            public void InvokeRegisterViewsAndServices()
            {
                base.RegisterViewsAndServices();
            }
        }
    }
}
