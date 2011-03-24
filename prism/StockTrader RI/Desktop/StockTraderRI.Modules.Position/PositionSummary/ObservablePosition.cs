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
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using StockTraderRI.Infrastructure.Interfaces;
using Microsoft.Practices.Composite.Events;
using Microsoft.Practices.Composite.Presentation.Events;
using StockTraderRI.Infrastructure;
using StockTraderRI.Infrastructure.Models;

namespace StockTraderRI.Modules.Position.PositionSummary
{
    public class ObservablePosition : IObservablePosition
    {
        private IAccountPositionService accountPositionService;
        private IMarketFeedService marketFeedService;

        public ObservableCollection<PositionSummaryItem> Items { get; private set; }

        public ObservablePosition(IAccountPositionService accountPositionService, IMarketFeedService marketFeedService, IEventAggregator eventAggregator)
        {
            this.Items = new ObservableCollection<PositionSummaryItem>();

            this.accountPositionService = accountPositionService;
            this.marketFeedService = marketFeedService;

            eventAggregator.GetEvent<MarketPricesUpdatedEvent>().Subscribe(MarketPricesUpdated, ThreadOption.UIThread);
           
            PopulateItems();

            this.accountPositionService.Updated += PositionSummaryItems_Updated;
        }

        public void MarketPricesUpdated(IDictionary<string, decimal> tickerSymbolsPrice)
        {
            foreach (PositionSummaryItem position in this.Items)
            {
                if (tickerSymbolsPrice.ContainsKey(position.TickerSymbol))
                {
                    position.CurrentPrice = tickerSymbolsPrice[position.TickerSymbol];
                }
            }
        }

        private void PositionSummaryItems_Updated(object sender, AccountPositionModelEventArgs e)
        {
            if (e.AcctPosition != null)
            {
                PositionSummaryItem positionSummaryItem = this.Items.First(p => p.TickerSymbol == e.AcctPosition.TickerSymbol);

                if (positionSummaryItem != null)
                {
                    positionSummaryItem.Shares = e.AcctPosition.Shares;
                    positionSummaryItem.CostBasis = e.AcctPosition.CostBasis;
                }
            }
        }

        private void PopulateItems()
        {
            PositionSummaryItem positionSummaryItem;
            foreach (AccountPosition accountPosition in this.accountPositionService.GetAccountPositions())
            {
                positionSummaryItem = new PositionSummaryItem(accountPosition.TickerSymbol, accountPosition.CostBasis, accountPosition.Shares, this.marketFeedService.GetPrice(accountPosition.TickerSymbol));
                this.Items.Add(positionSummaryItem);
            }
        }
    }
}
