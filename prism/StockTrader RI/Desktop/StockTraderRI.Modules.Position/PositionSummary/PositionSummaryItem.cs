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
using StockTraderRI.Infrastructure.Models;

namespace StockTraderRI.Modules.Position.PositionSummary
{
    public class PositionSummaryItem : INotifyPropertyChanged
    {
        private const string GAINLOSSPERCENTPROPERTYNAME = "GainLossPercent";
        private const string MARKETVALUEPROPERTYNAME = "MarketValue";

        public PositionSummaryItem(string tickerSymbol, decimal costBasis, long shares, decimal currentPrice)
        {
            TickerSymbol = tickerSymbol;
            CostBasis = costBasis;
            Shares = shares;
            CurrentPrice = currentPrice;
        }

        private string _tickerSymbol;

        public string TickerSymbol
        {
            get
            {
                return _tickerSymbol;
            }
            set
            {
                if (!value.Equals(_tickerSymbol))
                {
                    _tickerSymbol = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("TickerSymbol"));
                }
            }
        }


        private decimal _costBasis;

        public decimal CostBasis
        {
            get
            {
                return _costBasis;
            }
            set
            {
                if (!value.Equals(_costBasis))
                {
                    _costBasis = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("CostBasis"));
                    PropertyChanged(this, new PropertyChangedEventArgs(GAINLOSSPERCENTPROPERTYNAME));
                }
            }
        }


        private long _shares;

        public long Shares
        {
            get
            {
                return _shares;
            }
            set
            {
                if (!value.Equals(_shares))
                {
                    _shares = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("Shares"));
                    PropertyChanged(this, new PropertyChangedEventArgs(MARKETVALUEPROPERTYNAME));
                    PropertyChanged(this, new PropertyChangedEventArgs(GAINLOSSPERCENTPROPERTYNAME));
                }
            }
        }


        private decimal _currentPrice;

        public decimal CurrentPrice
        {
            get
            {
                return _currentPrice;
            }
            set
            {
                if (!value.Equals(_currentPrice))
                {
                    _currentPrice = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("CurrentPrice"));
                    PropertyChanged(this, new PropertyChangedEventArgs(MARKETVALUEPROPERTYNAME));
                    PropertyChanged(this, new PropertyChangedEventArgs(GAINLOSSPERCENTPROPERTYNAME));
                }
            }
        }

        public decimal MarketValue
        {
            get
            {
                return (_shares * _currentPrice);
            }
        }

        public decimal GainLossPercent
        {
            get
            {
                return ((CurrentPrice * Shares - CostBasis) * 100 / CostBasis);
            }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        #endregion
    }
}