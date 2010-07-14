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
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StockTraderRI.Modules.WatchList.Tests.Mocks;
using StockTraderRI.Modules.Watch.AddWatch;

namespace StockTraderRI.Modules.WatchList.Tests.AddWatch
{
    /// <summary>
    /// Summary description for AddWatchPresenterFixture
    /// </summary>
    [TestClass]
    public class AddWatchPresenterFixture
    {
	
        [TestMethod]
        public void PresenterTakesCommandFromWatchListServiceAndSetsIntoView()
        {
            MockWatchListService service = new MockWatchListService();
            MockAddWatchView view = new MockAddWatchView();
            
            AddWatchPresenter presenter = new AddWatchPresenter(view, service);

            Assert.AreSame(view.AddWatchCommand, service.AddWatchCommand);
            Assert.AreSame(view, presenter.View);
        }

    }
}
