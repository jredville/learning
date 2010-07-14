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
using StockTraderRI.Tests.AcceptanceTest.TestInfrastructure.MockModels;
using System.Data;
using System.Globalization;
using AcceptanceTestLibrary.ApplicationHelper;

namespace StockTraderRI.Tests.AcceptanceTest.TestInfrastructure
{
    public class MarketHistoryDataProvider : DataProviderBase<MarketHistoryItem>
    {
        public MarketHistoryDataProvider()
            : base()
        { }

        public override string GetDataFilePath()
        {
            return ConfigHandler.GetValue("MarketHistoryDataFile");
        }

        public override List<MarketHistoryItem> GetData()
        {
            DataSet ds = new DataSet();
            ds.Locale = CultureInfo.CurrentCulture;
            ds.ReadXml(GetDataFilePath());
            DataRow dr = null;

            List<MarketHistoryItem> history = new List<MarketHistoryItem>();
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                dr = ds.Tables[0].Rows[i];
                history.Add(
                    new MarketHistoryItem(
                        dr[TestDataInfrastructure.GetTestInputData("TickerSymbol")].ToString(),
                        DateTime.Parse(dr[TestDataInfrastructure.GetTestInputData("Date")].ToString(), CultureInfo.InvariantCulture),
                        decimal.Parse(dr[2].ToString(), CultureInfo.InvariantCulture)
                        ));
            }

            return history;
        }
    }
}
