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
using System.Xml.Linq;
using System.Text;
using StockTraderRI.Tests.AcceptanceTest.TestInfrastructure.MockModels;
using System.Data;
using System.Globalization;
using AcceptanceTestLibrary.ApplicationHelper;

namespace StockTraderRI.Tests.AcceptanceTest.TestInfrastructure.DataProvider.ModuleDataProviders
{
    public class NewsDataProvider: DataProviderBase<News>
    {
        public NewsDataProvider()
            : base()
        { }

        public override string GetDataFilePath()
        {
            return ConfigHandler.GetValue("NewsDataFile");
        }

        public override List<News> GetDataForId(string id)
        {
            List<News> news = new List<News>();

            XDocument xDoc = XDocument.Load(GetDataFilePath());
            foreach (XElement newsItem in xDoc.Descendants("NewsItems").Descendants("NewsItem")
                .Where(newsItem => newsItem.Attribute(TestDataInfrastructure.GetTestInputData("TickerSymbol")).Value.Equals(id)))
            {
                news.Add(
                    new News(
                        id,
                        newsItem.Attribute(TestDataInfrastructure.GetTestInputData("IconUri")).Value,
                        DateTime.Parse(newsItem.Attribute(TestDataInfrastructure.GetTestInputData("PublishedDate")).Value, CultureInfo.InvariantCulture),
                        newsItem.Elements("Title").ToList<XElement>()[0].Value,
                        newsItem.Elements("Body").ToList<XElement>()[0].Value
                        )
                    );
            }

            return news;
        }
    }
}
