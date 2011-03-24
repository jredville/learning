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
using System.ComponentModel;
using Microsoft.Practices.Composite.Events;
using StockTraderRI.Infrastructure;
using StockTraderRI.Infrastructure.Interfaces;
using StockTraderRI.Infrastructure.Models;

namespace StockTraderRI.Modules.Market.TrendLine
{
    public class TrendLinePresentationModel : ITrendLinePresentationModel, INotifyPropertyChanged
    {
        private readonly IMarketHistoryService _marketHistoryService;

        private string tickerSymbol;

        private MarketHistoryCollection historyCollection;

        public TrendLinePresentationModel(ITrendLineView view, IMarketHistoryService marketHistoryService, IEventAggregator eventAggregator)
        {
            this.View = view;
            this.View.Model = this;
            this._marketHistoryService = marketHistoryService;
            eventAggregator.GetEvent<TickerSymbolSelectedEvent>().Subscribe(this.TickerSymbolChanged);
        }

        public void TickerSymbolChanged(string newTickerSymbol)
        {
            MarketHistoryCollection newHistoryCollection = this._marketHistoryService.GetPriceHistory(newTickerSymbol);

            this.TickerSymbol = newTickerSymbol;
            this.HistoryCollection = newHistoryCollection;
        }

        #region ITrendLinePresentationModel Members

        public ITrendLineView View { get; set; }

        public string TickerSymbol
        {
            get
            {
                return this.tickerSymbol;
            }
            set
            {
                if (this.tickerSymbol != value)
                {
                    this.tickerSymbol = value;
                    this.InvokePropertyChanged("TickerSymbol");
                }
            }
        }

        public MarketHistoryCollection HistoryCollection
        {
            get
            {
                return historyCollection;
            }
            private set
            {
                if (this.historyCollection != value)
                {
                    this.historyCollection = value;
                    this.InvokePropertyChanged("HistoryCollection");
                }
            }
        }

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        private void InvokePropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler Handler = PropertyChanged;
            if (Handler != null) Handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
