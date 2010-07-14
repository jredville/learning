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
using AcceptanceTestLibrary.ApplicationHelper;

namespace StockTraderRI.Tests.AcceptanceTest.TestInfrastructure
{
    public class TestDataInfrastructure
    {
        public int GetCount<T, TEntity>()
            where T : IDataProvider<TEntity>, new()
        {
            return new T().GetCount();
        }

        public List<TEntity> GetData<T, TEntity>()
            where T : IDataProvider<TEntity>, new()
        {
            return new T().GetData();
        }

        public List<TEntity> GetDataForId<T, TEntity>(string id)
            where T : IDataProvider<TEntity>, new()
        {
            return new T().GetDataForId(id);
        }
        public static string GetTestInputData(string key)
        {
            ResXConfigHandler testInputHandler = new ResXConfigHandler(ConfigHandler.GetValue("TestDataInputFile"));
            return testInputHandler.GetValue(key);
        }

        public static string GetControlId(string key)
        {
            ResXConfigHandler testInputHandler = new ResXConfigHandler(ConfigHandler.GetValue("ControlIdentifiersFile"));
            return testInputHandler.GetValue(key);
        }
    }
}
