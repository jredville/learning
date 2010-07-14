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
using Microsoft.Practices.Composite.Events;
using StockTraderRI.Modules.Position.Interfaces;
using StockTraderRI.Modules.Position.PositionSummary;

namespace StockTraderRI.Modules.Position.Tests.Mocks
{
    class MockPositionSummaryView : IPositionSummaryView
    {
        public IPositionSummaryPresentationModel Model { get; set; }

        public event EventHandler<DataEventArgs<string>> TickerSymbolSelected = delegate { };

        internal void SelectFUND0Row()
        {
            TickerSymbolSelected(this, new DataEventArgs<string>("FUND0"));
        }
    }
}