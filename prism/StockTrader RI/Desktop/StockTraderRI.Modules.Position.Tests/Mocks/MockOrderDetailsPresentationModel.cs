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
using Microsoft.Practices.Composite.Presentation.Commands;
using StockTraderRI.Infrastructure;
using StockTraderRI.Modules.Position.Models;
using StockTraderRI.Modules.Position.Orders;

namespace StockTraderRI.Modules.Position.Tests.Mocks
{
    public class MockOrderDetailsPresentationModel : IOrderDetailsPresentationModel
    {
        public bool DisposeCalled;

        public MockOrderDetailsPresentationModel()
        {
            View = new MockOrderDetailsView();
        }

        public event EventHandler CloseViewRequested;

        public IOrderDetailsView View { get; set; }

        #region IDisposable Members

        public void Dispose()
        {
            DisposeCalled = true;
        }

        #endregion

        internal void RaiseCloseViewRequested()
        {
            CloseViewRequested(this, EventArgs.Empty);
        }

        public TransactionInfo TransactionInfo { get; set; }

        public TransactionType TransactionType { get; set; }

        public string TickerSymbol { get; set;}

        public int? Shares { get; set;}

        public decimal? StopLimitPrice { get; set; }

        public DelegateCommand<object> SubmitCommand { get; set;}

        public DelegateCommand<object> CancelCommand { get; set;}

        #region IOrderDetailsPresentationModel Members

        #endregion
    }
}
