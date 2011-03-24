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
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace StockTraderRI.Tests.AcceptanceTest.TestInfrastructure.MockModels
{
    [Serializable]
    [XmlRoot("MarketItem")]
    public class Market
    {
        private string symbol;
        private decimal marketValue;
        private long volume;

        public Market() { }

        public Market(string symbol, decimal value)
        {
            this.symbol = symbol;
            this.marketValue = value;
        }

        public Market(string symbol, decimal value, long volume) 
        {
            this.symbol = symbol;
            this.marketValue = value;
            this.volume = volume;
        }

        [XmlAttribute("TickerSymbol")]
        public string TickerSymbol
        {
            get { return this.symbol; }
            set { this.symbol = value; }
        }

        [XmlAttribute("LastPrice")]
        public decimal LastPrice
        {
            get { return this.marketValue; }
            set { this.marketValue = value; }
        }

        [XmlAttribute("Volume")]
        public long Volume
        {
            get { return this.volume; }
            set { this.volume = value; }
        }
    }
}
