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
using System.ComponentModel;
using StockTraderRI.Infrastructure.Interfaces;
using StockTraderRI.Infrastructure.Models;
using StockTraderRI.Modules.News.Controllers;

namespace StockTraderRI.Modules.News.Article
{

    public class ArticlePresentationModel : IArticlePresentationModel, INotifyPropertyChanged
    {
        private IList<NewsArticle> articles;
        private NewsArticle selectedArticle;
        private readonly INewsFeedService newsFeedService;
        public event PropertyChangedEventHandler PropertyChanged;

        public ArticlePresentationModel(IArticleView view, INewsFeedService newsFeedService)
        {
            View = view;
            View.Model = this;
            this.newsFeedService = newsFeedService;
            View.ShowNewsReader += View_ShowNewsReader;
        }

        public IArticleView View { get; private set; }

        public INewsController Controller { get; set; }

        public void SetTickerSymbol(string companySymbol)
        {
            this.Articles = newsFeedService.GetNews(companySymbol);
        }


        public NewsArticle SelectedArticle
        {
            get { return this.selectedArticle; }
            set
            {
                if (this.selectedArticle != value)
                {
                    this.selectedArticle = value;
                    OnPropertyChanged("SelectedArticle");
                    this.SelectedArticleChanged();
                }
            }
        }
        
        public IList<NewsArticle> Articles
        {
            get { return this.articles; }
            private set
            {
                if (this.articles != value)
                {
                    this.articles = value;
                    OnPropertyChanged("Articles");
                }
            }
        }

        private void View_ShowNewsReader(object sender, EventArgs e)
        {
            this.Controller.ShowNewsReader();
        }

        private void SelectedArticleChanged()
        {
            this.Controller.CurrentNewsArticleChanged(this.SelectedArticle);
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
