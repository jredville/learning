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
using Microsoft.Practices.Composite.Events;
using Microsoft.Practices.Composite.Presentation.Events;
using Microsoft.Practices.Composite.Regions;
using StockTraderRI.Infrastructure;
using StockTraderRI.Infrastructure.Models;
using StockTraderRI.Modules.News.Article;

namespace StockTraderRI.Modules.News.Controllers
{
    public class NewsController : INewsController
    {
        private readonly IRegionManager regionManager;
        private readonly IArticlePresentationModel articlePresentationModel;
        private readonly IEventAggregator eventAggregator;
        private readonly INewsReaderPresenter readerPresenter;
        private readonly IRegion shellRegion;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "newsReader")]
        public NewsController(IRegionManager regionManager, IArticlePresentationModel articlePresentationModel, IEventAggregator eventAggregator, INewsReaderPresenter newsReaderPresenter)
        {
            this.regionManager = regionManager;
            this.articlePresentationModel = articlePresentationModel;
            this.eventAggregator = eventAggregator;
            this.articlePresentationModel.Controller = this;

            this.readerPresenter = newsReaderPresenter;

            this.shellRegion = this.regionManager.Regions[RegionNames.SecondaryRegion];
            this.shellRegion.Add(this.readerPresenter.View);
        }

        public void Run()
        {
            this.regionManager.Regions[RegionNames.ResearchRegion].Add(articlePresentationModel.View);
            eventAggregator.GetEvent<TickerSymbolSelectedEvent>().Subscribe(ShowNews, ThreadOption.UIThread);
        }

        public void ShowNews(string companySymbol)
        {
            this.articlePresentationModel.SetTickerSymbol(companySymbol);
        }

        public void CurrentNewsArticleChanged(NewsArticle article)
        {
            this.readerPresenter.SetNewsArticle(article);
        }

        public void ShowNewsReader()
        {
            this.shellRegion.Activate(this.readerPresenter.View);
        }
    }
}
