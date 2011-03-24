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
using StockTraderRI.Modules.Position.Models;

namespace StockTraderRI.Modules.Position.Orders
{
    public partial class OrderCompositePresentationModel : INotifyPropertyChanged
    {
        partial void SetTransactionInfo(TransactionInfo transactionInfo)
        {
            //This instance of TransactionInfo acts as a "shared model" between this view and the order details view.
            //The scenario says that these 2 views are decoupled, so they don't share the presentation model, they are only tied
            //with this TransactionInfo
            this.orderDetailsPresentationModel.TransactionInfo = transactionInfo;
            this.HeaderInfo = transactionInfo.TickerSymbol;
        }

        public string HeaderInfo
        {
            get
            {
                return (string)GetValue(HeaderInfoProperty);
            }
            set
            {
                SetValue(HeaderInfoProperty, value);
                InvokePropertyChanged(new PropertyChangedEventArgs("HeaderInfo"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void InvokePropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChangedEventHandler Handler = PropertyChanged;
            if (Handler != null) Handler(this, e);
        }
    }
}
