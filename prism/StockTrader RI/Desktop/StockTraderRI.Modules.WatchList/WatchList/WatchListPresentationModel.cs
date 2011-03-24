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
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using Microsoft.Practices.Composite.Events;
using Microsoft.Practices.Composite.Presentation.Commands;
using Microsoft.Practices.Composite.Presentation.Events;
using StockTraderRI.Infrastructure;
using StockTraderRI.Infrastructure.Interfaces;
using StockTraderRI.Modules.Watch.Properties;
using StockTraderRI.Modules.Watch.Services;

namespace StockTraderRI.Modules.Watch.WatchList
{
    public class WatchListPresentationModel : IWatchListPresentationModel, INotifyPropertyChanged, IHeaderInfoProvider<string>
    {
        private readonly IMarketFeedService marketFeedService;
        private readonly IEventAggregator eventAggregator;
        private readonly ObservableCollection<string> watchList;
        private ICommand removeWatchCommand;
        private ObservableCollection<WatchItem> watchListItems;
        private WatchItem currentWatchItem;

        public WatchListPresentationModel(IWatchListView view, IWatchListService watchListService, IMarketFeedService marketFeedService, IEventAggregator eventAggregator)
        {
            this.View = view;
            this.HeaderInfo = Resources.WatchListTitle;
            this.WatchListItems = new ObservableCollection<WatchItem>();
            this.View.SetModel(this);

            this.marketFeedService = marketFeedService;

            this.watchList = watchListService.RetrieveWatchList();
            this.watchList.CollectionChanged += delegate { this.PopulateWatchItemsList(this.watchList); };
            this.PopulateWatchItemsList(this.watchList);

            this.eventAggregator = eventAggregator;
            this.eventAggregator.GetEvent<MarketPricesUpdatedEvent>().Subscribe(this.MarketPricesUpdated, ThreadOption.UIThread);
            this.RemoveWatchCommand = new DelegateCommand<string>(this.RemoveWatch);
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public ObservableCollection<WatchItem> WatchListItems
        {
            get
            {
                return this.watchListItems;
            }

            set
            {
                if (this.watchListItems != value)
                {
                    this.watchListItems = value;
                    this.OnPropertyChanged("WatchListItems");
                }
            }
        }

        public WatchItem CurrentWatchItem
        {
            get
            {
                return this.currentWatchItem;
            }

            set
            {
                if (value != null && this.currentWatchItem != value)
                {
                    this.currentWatchItem = value;
                    this.OnPropertyChanged("CurrentWatchItem");
                    this.eventAggregator.GetEvent<TickerSymbolSelectedEvent>().Publish(this.currentWatchItem.TickerSymbol);
                }
            }
        }

        public string HeaderInfo { get; set; }

        public IWatchListView View { get; private set; }

        public ICommand RemoveWatchCommand
        {
            get
            {
                return this.removeWatchCommand;
            }

            private set
            {
                this.removeWatchCommand = value;
                this.OnPropertyChanged("RemoveWatchCommand");
            }
        }

        public void MarketPricesUpdated(IDictionary<string, decimal> updatedPrices)
        {
            foreach (WatchItem watchItem in this.WatchListItems)
            {
                if (updatedPrices.ContainsKey(watchItem.TickerSymbol))
                {
                    watchItem.CurrentPrice = updatedPrices[watchItem.TickerSymbol];
                }
            }
        }

        private void RemoveWatch(string tickerSymbol)
        {
            this.watchList.Remove(tickerSymbol);
        }

        private void PopulateWatchItemsList(IEnumerable<string> watchItemsList)
        {
            this.WatchListItems.Clear();
            foreach (string tickerSymbol in watchItemsList)
            {
                decimal? currentPrice;
                try
                {
                    currentPrice = this.marketFeedService.GetPrice(tickerSymbol);
                }
                catch (ArgumentException)
                {
                    currentPrice = null;
                }

                this.WatchListItems.Add(new WatchItem(tickerSymbol, currentPrice));
            }
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
