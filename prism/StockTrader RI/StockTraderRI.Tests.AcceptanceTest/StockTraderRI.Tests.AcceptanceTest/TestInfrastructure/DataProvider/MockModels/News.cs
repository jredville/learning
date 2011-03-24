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
    [XmlRoot("NewsItem")]
    public class News
    {
        private string symbol;
        private string iconUriPath;
        private string title;
        private string body;
        private DateTime publishedDate;

        public News() { }

        public News(string symbol)
            : this(symbol, null, String.Empty, String.Empty)
        { }

        public News(string symbol, string title)
            : this(symbol, null, title, String.Empty)
        { }

        public News(string symbol, string title, string body)
            : this(symbol, null, title, body)
        { }

        public News(string symbol, string iconUriPath, string title, string body)
            : this(symbol, iconUriPath, DateTime.MinValue, title, body)
        { }

        public News(string symbol, string iconUriPath, DateTime publishedDate, string title, string body)
        {
            this.symbol = symbol;
            this.iconUriPath = iconUriPath;
            this.publishedDate = publishedDate;
            this.title = title;
            this.body = body;
        }

        [XmlAttribute("TickerSymbol")]
        public string TickerSymbol
        {
            get { return this.symbol; }
            set { this.symbol = value; }
        }

        [XmlAttribute("IconUri")]
        public string IconUriPath
        {
            get { return this.iconUriPath; }
            set { this.iconUriPath = value; }
        }

        [XmlElement("Title")]
        public string Title
        {
            get { return this.title; }
            set { this.title = value; }
        }

        [XmlElement("Body")]
        public string Body
        {
            get { return this.body; }
            set { this.body = value; }
        }

        public DateTime PublishedDate
        {
            get { return this.publishedDate; }
            set { this.publishedDate = value; }
        }
    }
}
