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
using Microsoft.Practices.Composite.Modularity;
using Microsoft.Practices.Unity;
using StockTraderRI.Infrastructure.Interfaces;
using StockTraderRI.Modules.News.Article;
using StockTraderRI.Modules.News.Controllers;
using StockTraderRI.Modules.News.Services;

namespace StockTraderRI.Modules.News
{
    public class NewsModule : IModule
    {
        private readonly IUnityContainer container;

        public NewsModule(IUnityContainer container)
        {
            this.container = container;
        }

        public void Initialize()
        {
            this.RegisterViewsAndServices();

            INewsController controller = this.container.Resolve<INewsController>();
            controller.Run();
        }

        protected void RegisterViewsAndServices()
        {
            this.container.RegisterType<INewsController, NewsController>(new ContainerControlledLifetimeManager());
            this.container.RegisterType<IArticleView, ArticleView>();
            this.container.RegisterType<IArticlePresentationModel, ArticlePresentationModel>();
            this.container.RegisterType<INewsFeedService, NewsFeedService>(new ContainerControlledLifetimeManager());
            this.container.RegisterType<INewsReaderView, NewsReader>();
            this.container.RegisterType<INewsReaderPresenter, NewsReaderPresenter>();
        }
    }
}
