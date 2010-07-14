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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StockTraderRI.Infrastructure;
using StockTraderRI.Infrastructure.Interfaces;
using StockTraderRI.Modules.Position.Interfaces;
using StockTraderRI.Modules.Position.Models;
using StockTraderRI.Modules.Position.Orders;
using StockTraderRI.Modules.Position.Tests.Mocks;

namespace StockTraderRI.Modules.Position.Tests.Orders
{
    /// <summary>
    /// Summary description for OrderCompositePresenterFixture
    /// </summary>
    [TestClass]
    public class OrderCompositePresentationModelFixture
    {
        [TestMethod]
        public void ShouldCreateOrderDetailsPresenter()
        {
            var detailsPresenter = new MockOrderDetailsPresentationModel();
            IOrderCompositeView compositeView = new MockOrderCompositeView();

            var composite = new OrderCompositePresentationModel(compositeView, detailsPresenter, new MockOrderCommandsView());

            composite.TransactionInfo = new TransactionInfo("FXX01", TransactionType.Sell);

            Assert.IsNotNull(detailsPresenter.TransactionInfo);
        }

        [TestMethod]
        public void ShouldAddDetailsViewAndControlsViewToContentArea()
        {
            MockOrderCompositeView compositeView = new MockOrderCompositeView();
            var detailsPresenter = new MockOrderDetailsPresentationModel();

            var composite = new OrderCompositePresentationModel(compositeView, detailsPresenter, new MockOrderCommandsView());

            Assert.AreEqual(detailsPresenter.View, composite.OrderDetailsView);
            Assert.IsNotNull(composite.OrderCommandsView as IOrderCommandsView);
        }

        [TestMethod]
        public void PresenterExposesChildOrderPresentersCloseRequested()
        {
            var detailsPresenter = new MockOrderDetailsPresentationModel();
            MockOrderCompositeView compositeView = new MockOrderCompositeView();

            var composite = new OrderCompositePresentationModel(compositeView, detailsPresenter, new MockOrderCommandsView());

            var closeViewRequestedFired = false;
            composite.CloseViewRequested += delegate
                                                {
                                                    closeViewRequestedFired = true;
                                                };

            detailsPresenter.RaiseCloseViewRequested();

            Assert.IsTrue(closeViewRequestedFired);

        }

        [TestMethod]
        public void ShouldPassModelToView()
        {
            var view = new MockOrderCompositeView();
            var composite = new OrderCompositePresentationModel(view, new MockOrderDetailsPresentationModel(), new MockOrderCommandsView());

            composite.TransactionInfo = new TransactionInfo("FXX01", TransactionType.Sell);

            Assert.IsNotNull(view);
            Assert.AreEqual(composite, view.Model);
        }

        [TestMethod]
        public void TransactionInfoAndSharesAndCommandsAreTakenFromOrderDetails()
        {
            var orderDetailsPM = new MockOrderDetailsPresentationModel();
            var composite = new OrderCompositePresentationModel(new MockOrderCompositeView(), orderDetailsPM, new MockOrderCommandsView());
            orderDetailsPM.Shares = 100;

            Assert.AreEqual(orderDetailsPM.Shares, composite.Shares);
            Assert.AreSame(orderDetailsPM.SubmitCommand, composite.SubmitCommand);
            Assert.AreSame(orderDetailsPM.CancelCommand, composite.CancelCommand);
            Assert.AreSame(orderDetailsPM.TransactionInfo, composite.TransactionInfo);
        }

#if !SILVERLIGHT
        // In the Silverlight version of the RI, header binding is done purely in the XAML separate textblocks (no MultiBinding available).
        [TestMethod]
        public void ShouldSetHeaderInfo()
        {
            var composite = new OrderCompositePresentationModel(new MockOrderCompositeView(), new MockOrderDetailsPresentationModel(), new MockOrderCommandsView());

            composite.TransactionInfo = new TransactionInfo("FXX01", TransactionType.Sell);

            Assert.IsNotNull(composite.HeaderInfo);
            Assert.IsTrue(composite.HeaderInfo.Contains("FXX01"));
            Assert.IsTrue(composite.HeaderInfo.Contains("Sell"));
            Assert.AreEqual("Sell FXX01", composite.HeaderInfo);
        }

        [TestMethod]
        public void ShouldUpdateHeaderInfoWhenUpdatingTransactionInfo()
        {
            var orderDetailsPM = new MockOrderDetailsPresentationModel();
            var composite = new OrderCompositePresentationModel(new MockOrderCompositeView(), orderDetailsPM, new MockOrderCommandsView());

            composite.TransactionInfo = new TransactionInfo("FXX01", TransactionType.Sell);

            orderDetailsPM.TransactionInfo.TickerSymbol = "NEW_SYMBOL";
            Assert.AreEqual("Sell NEW_SYMBOL", composite.HeaderInfo);

            orderDetailsPM.TransactionInfo.TransactionType = TransactionType.Buy;
            Assert.AreEqual("Buy NEW_SYMBOL", composite.HeaderInfo);
        }
#endif
    }

    internal class MockOrderCommandsView : IOrderCommandsView
    {
        public void SetModel(object model)
        {
        }
    }

    internal class MockOrderCompositeView : IOrderCompositeView
    {
        public IOrderCompositePresentationModel Model { get; private set; }

        public void SetModel(IOrderCompositePresentationModel model)
        {
            this.Model = model;
        }
    }
}
