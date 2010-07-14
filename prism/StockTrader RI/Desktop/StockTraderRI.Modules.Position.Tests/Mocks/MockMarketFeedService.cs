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
using StockTraderRI.Infrastructure.Interfaces;

namespace StockTraderRI.Modules.Position.Tests.Mocks
{
    public class MockMarketFeedService : IMarketFeedService
    {

        Dictionary<string, decimal> feedData = new Dictionary<string, decimal>();

        internal void SetPrice(string tickerSymbol, decimal price)
        {
            feedData.Add(tickerSymbol, price);
        }


        internal void UpdatePrice(string tickerSymbol, decimal newPrice, long volume)
        {
            feedData[tickerSymbol] = newPrice;
        }

        #region IMarketFeedService Members

        public decimal GetPrice(string tickerSymbol)
        {
            if (feedData.ContainsKey(tickerSymbol))
                return feedData[tickerSymbol];
            return 0m;
        }

        public long GetVolume(string tickerSymbol)
        {
            throw new NotImplementedException();
        }

        #endregion

        public bool SymbolExists(string tickerSymbol)
        {
            throw new NotImplementedException();
        }
    }
}
