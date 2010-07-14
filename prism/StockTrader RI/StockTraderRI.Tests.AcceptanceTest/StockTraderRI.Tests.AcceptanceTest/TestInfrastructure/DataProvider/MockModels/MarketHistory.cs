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
    [XmlRoot("MarketHistoryItem")]
    public class MarketHistoryItem
    {
        private string symbol;
        private DateTime date;
        private decimal item;

        public MarketHistoryItem()
        { }

        public MarketHistoryItem(string symbol, DateTime date, decimal item)
        {
            this.symbol = symbol;
            this.date = date;
            this.item = item;
        }

        [XmlAttribute("TickerSymbol")]
        public string TickerSymbol 
        {
            get { return this.symbol; }
            set { this.symbol = value; }
        }

        [XmlAttribute("Date")]
        public DateTime Date
        {
            get { return this.date; }
            set { this.date = value; }
        }

        [XmlElement("MarketHistoryItem")]
        public decimal MarketItem
        {
            get { return this.item; }
            set { this.item = value; }
        }
    }
}
