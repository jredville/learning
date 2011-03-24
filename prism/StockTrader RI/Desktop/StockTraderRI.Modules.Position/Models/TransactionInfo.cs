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
using StockTraderRI.Infrastructure;

namespace StockTraderRI.Modules.Position.Models
{
    public class TransactionInfo : INotifyPropertyChanged
    {
        private string tickerSymbol;
        private TransactionType transactionType;

        public TransactionInfo()
        {
        }

        public TransactionInfo(string tickerSymbol, TransactionType transactionType)
        {
            this.tickerSymbol = tickerSymbol;
            this.transactionType = transactionType;
        }

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

        public TransactionType TransactionType
        {
            get
            {
                return this.transactionType;
            }

            set
            {
                if (this.transactionType != value)
                {
                    this.transactionType = value;
                    this.InvokePropertyChanged("TransactionType");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void InvokePropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler Handler = PropertyChanged;
            if (Handler != null) Handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}