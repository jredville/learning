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
using StockTraderRI.Infrastructure.Interfaces;
using StockTraderRI.Infrastructure.Models;

namespace StockTraderRI.Modules.Market.Tests.Mocks
{
    internal class MockMarketHistoryService : IMarketHistoryService
    {
        public bool GetPriceHistoryCalled;
        public string GetPriceHistoryArgument;
        public MarketHistoryCollection Data = new MarketHistoryCollection();

        public MockMarketHistoryService()
        {
            Data.Add(new MarketHistoryItem { DateTimeMarker = new DateTime(2008, 1, 1), Value = 10.00m });
            Data.Add(new MarketHistoryItem { DateTimeMarker = new DateTime(2008, 6, 1), Value = 15.00m });
        }

        public MarketHistoryCollection GetPriceHistory(string tickerSymbol)
        {
            GetPriceHistoryCalled = true;
            GetPriceHistoryArgument = tickerSymbol;

            return Data;
        }
    }
}
