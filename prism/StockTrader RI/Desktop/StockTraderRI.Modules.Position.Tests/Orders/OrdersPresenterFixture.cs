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
using Microsoft.Practices.Unity;
using StockTraderRI.Modules.Position.Tests.Mocks;
using StockTraderRI.Modules.Position.Interfaces;
using StockTraderRI.Modules.Position.Orders;

namespace StockTraderRI.Modules.Position.Tests.Orders
{
    /// <summary>
    /// Summary description for OrdersPresenterFixture
    /// </summary>
    [TestClass]
    public class OrdersPresenterFixture
    {
        MockOrdersView view;

        [TestInitialize]
        public void SetUp()
        {
            view = new MockOrdersView();
        }

        [TestMethod]
        public void CanInitPresenter()
        {
            OrdersPresentationModel presentationModel = CreatePresenter();

            Assert.AreEqual<IOrdersView>(view, presentationModel.View);
        }

        private OrdersPresentationModel CreatePresenter()
        {
            return new OrdersPresentationModel(view);
        }
    }
}
