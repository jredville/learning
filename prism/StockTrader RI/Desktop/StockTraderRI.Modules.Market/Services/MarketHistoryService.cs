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
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using StockTraderRI.Infrastructure.Interfaces;
using StockTraderRI.Infrastructure.Models;
using StockTraderRI.Modules.Market.Properties;

namespace StockTraderRI.Modules.Market.Services
{
    public class MarketHistoryService : IMarketHistoryService
    {
        private Dictionary<string, MarketHistoryCollection> _marketHistory;

        public MarketHistoryService()
        {
            InitializeMarketHistory();
        }

        private void InitializeMarketHistory()
        {
            XDocument document = XDocument.Parse(Resources.MarketHistory);

            _marketHistory = document.Descendants("MarketHistoryItem")
                .GroupBy(x => x.Attribute("TickerSymbol").Value,
                         x => new MarketHistoryItem
                                  {
                                      DateTimeMarker = DateTime.Parse(x.Attribute("Date").Value, CultureInfo.InvariantCulture),
                                      Value = Decimal.Parse(x.Value, NumberStyles.Float, CultureInfo.InvariantCulture)
                                  })
                .ToDictionary(group => group.Key, group => new MarketHistoryCollection(group));
        }

        public MarketHistoryCollection GetPriceHistory(string tickerSymbol)
        {
            MarketHistoryCollection items = _marketHistory[tickerSymbol];
            return items;
        }
    }
}
