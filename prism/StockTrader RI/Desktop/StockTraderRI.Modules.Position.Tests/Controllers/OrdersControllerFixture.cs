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
using System.Windows.Input;
using Microsoft.Practices.Composite.Regions;
using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StockTraderRI.Infrastructure;
using StockTraderRI.Modules.Position.Controllers;
using StockTraderRI.Modules.Position.Interfaces;
using StockTraderRI.Modules.Position.Models;
using StockTraderRI.Modules.Position.Orders;
using StockTraderRI.Modules.Position.Tests.Mocks;
using StockTraderRI.Modules.Position.Tests.Orders;
using StockTraderRI.Infrastructure.Interfaces;
using Microsoft.Practices.Composite.TestSupport;

namespace StockTraderRI.Modules.Position.Tests.Controllers
{
    [TestClass]
    public class OrdersControllerFixture
    {
        private MockRegionManager regionManager;
        private MockRegion ordersRegion;

        [TestInitialize]
        public void SetUp()
        {
            regionManager = new MockRegionManager();
            regionManager.Regions.Add("ActionRegion", new MockRegion());
            ordersRegion = new MockRegion();
            regionManager.Regions.Add("OrdersRegion", ordersRegion);
        }

        [TestMethod]
        public void BuyAndSellCommandsInvokeController()
        {
            var container = new MockUnityContainer();
            container.ResolveBag.Add(typeof(IOrdersPresentationModel), new MockOrdersPresentationModel());
            container.ResolveBag.Add(typeof(IOrderCompositePresentationModel), new MockOrderCompositePresentationModel());
            
            var controller = new TestableOrdersController(regionManager, container, new MockStockTraderRICommandProxy(), null);
            controller.BuyCommand.Execute("STOCK01");

            Assert.AreEqual("STOCK01", controller.StartOrderArgumentTickerSymbol);
            Assert.AreEqual(TransactionType.Buy, controller.StartOrderArgumentTransactionType);

            // Set new CompositePresentationModel to simulate resolution of new instance.
            container.ResolveBag[typeof (IOrderCompositePresentationModel)] = new MockOrderCompositePresentationModel();
            controller.SellCommand.Execute("STOCK02");

            Assert.AreEqual("STOCK02", controller.StartOrderArgumentTickerSymbol);
            Assert.AreEqual(TransactionType.Sell, controller.StartOrderArgumentTransactionType);
        }

        [TestMethod]
        public void ControllerAddsViewIfNotPresent()
        {
            var container = new MockUnityContainer();
            container.ResolveBag.Add(typeof(IOrdersPresentationModel), new MockOrdersPresentationModel());
            container.ResolveBag.Add(typeof(IOrderCompositePresentationModel), new MockOrderCompositePresentationModel());
            var collapsibleRegion = (MockRegion) regionManager.Regions["ActionRegion"];

            var controller = new TestableOrdersController(regionManager, container, new MockStockTraderRICommandProxy(), null);

            Assert.AreEqual<int>(0, collapsibleRegion.AddedViews.Count);
            controller.InvokeStartOrder(TransactionType.Buy, "STOCK01");
            Assert.AreEqual<int>(1, collapsibleRegion.AddedViews.Count);
        }

        [TestMethod]
        public void ControllerAddsANewOrderOnStartOrder()
        {
            var container = new MockUnityContainer();
            container.ResolveBag.Add(typeof(IOrdersPresentationModel), new MockOrdersPresentationModel());
            container.ResolveBag.Add(typeof(IOrderCompositePresentationModel), new MockOrderCompositePresentationModel());

            var controller = new TestableOrdersController(regionManager, container, new MockStockTraderRICommandProxy(), null);

            Assert.AreEqual<int>(0, ordersRegion.AddedViews.Count);
            controller.InvokeStartOrder(TransactionType.Buy, "STOCK01");
            Assert.AreEqual<int>(1, ordersRegion.AddedViews.Count);
        }

        [TestMethod]
        public void NewOrderIsShownOrder()
        {
            var container = new MockUnityContainer();
            container.ResolveBag.Add(typeof(IOrdersPresentationModel), new MockOrdersPresentationModel());
            container.ResolveBag.Add(typeof(IOrderCompositePresentationModel), new MockOrderCompositePresentationModel());

            var controller = new TestableOrdersController(regionManager, container, new MockStockTraderRICommandProxy(), null);

            Assert.AreEqual<int>(0, ordersRegion.AddedViews.Count);
            controller.InvokeStartOrder(TransactionType.Buy, "STOCK01");
            Assert.AreSame(ordersRegion.SelectedItem, ordersRegion.AddedViews[0]);
        }

        [TestMethod]
        public void StartOrderHooksInstanceCommandsToGlobalSaveAllAndCancelAllCommands()
        {
            var container = new MockUnityContainer();
            var orderCompositePresenter = new MockOrderCompositePresentationModel();
            container.ResolveBag.Add(typeof(IOrdersPresentationModel), new MockOrdersPresentationModel());
            container.ResolveBag.Add(typeof(IOrderCompositePresentationModel), orderCompositePresenter);

            var commandProxy = new MockStockTraderRICommandProxy();

            var controller = new TestableOrdersController(regionManager, container, commandProxy, null);
            controller.InvokeStartOrder(TransactionType.Buy, "STOCK01");

            Assert.IsFalse(orderCompositePresenter.MockSubmitCommand.ExecuteCalled);
            commandProxy.SubmitAllOrdersCommand.Execute(null);
            Assert.IsTrue(orderCompositePresenter.MockSubmitCommand.ExecuteCalled);

            Assert.IsFalse(orderCompositePresenter.MockCancelCommand.ExecuteCalled);
            commandProxy.CancelAllOrdersCommand.Execute(null);
            Assert.IsTrue(orderCompositePresenter.MockCancelCommand.ExecuteCalled);
        }
      
        [TestMethod]
        public void StartOrderHooksInstanceCommandsToGlobalSaveAndCancelCommands()
        {
            var container = new MockUnityContainer();
            var orderCompositePresenter = new MockOrderCompositePresentationModel();
            container.ResolveBag.Add(typeof(IOrdersPresentationModel), new MockOrdersPresentationModel());
            container.ResolveBag.Add(typeof(IOrderCompositePresentationModel), orderCompositePresenter);

            var commandProxy = new MockStockTraderRICommandProxy();

            var controller = new TestableOrdersController(regionManager, container, commandProxy, null);
            controller.InvokeStartOrder(TransactionType.Buy, "STOCK01");

            Assert.IsFalse(orderCompositePresenter.MockSubmitCommand.ExecuteCalled);
            commandProxy.SubmitOrderCommand.Execute(null);
            Assert.IsTrue(orderCompositePresenter.MockSubmitCommand.ExecuteCalled);

            Assert.IsFalse(orderCompositePresenter.MockCancelCommand.ExecuteCalled);
            commandProxy.CancelOrderCommand.Execute(null);
            Assert.IsTrue(orderCompositePresenter.MockCancelCommand.ExecuteCalled);
        }

        [TestMethod]
        public void OnCloseViewRequestedTheControllerUnhooksGlobalCommands()
        {
            var container = new MockUnityContainer();
            var orderCompositePresenter = new MockOrderCompositePresentationModel();
            container.ResolveBag.Add(typeof(IOrdersPresentationModel), new MockOrdersPresentationModel());
            container.ResolveBag.Add(typeof(IOrderCompositePresentationModel), orderCompositePresenter);
            var commandProxy = new MockStockTraderRICommandProxy();

            var controller = new TestableOrdersController(regionManager, container, commandProxy, null);
            controller.InvokeStartOrder(TransactionType.Buy, "STOCK01");

            Assert.AreEqual(1, ordersRegion.AddedViews.Count);
            orderCompositePresenter.RaiseCloseViewRequested();

            Assert.AreEqual(0, ordersRegion.AddedViews.Count);

            commandProxy.SubmitAllOrdersCommand.Execute(null);
            Assert.IsFalse(orderCompositePresenter.MockSubmitCommand.ExecuteCalled);

            commandProxy.CancelAllOrdersCommand.Execute(null);
            Assert.IsFalse(orderCompositePresenter.MockCancelCommand.ExecuteCalled);

            commandProxy.SubmitOrderCommand.Execute(null);
            Assert.IsFalse(orderCompositePresenter.MockSubmitCommand.ExecuteCalled);

            commandProxy.CancelOrderCommand.Execute(null);
            Assert.IsFalse(orderCompositePresenter.MockCancelCommand.ExecuteCalled);
        }

        [TestMethod]
        public void StartOrderCreatesCompositePMAndPassesCorrectInitInfo()
        {
            IUnityContainer container = new UnityContainer();
            container.RegisterType<IOrdersPresentationModel, MockOrdersPresentationModel>();
            var presentationModel = new MockOrderCompositePresentationModel();
            container.RegisterInstance<IOrderCompositePresentationModel>(presentationModel);

            var controller = new TestableOrdersController(regionManager, container, new MockStockTraderRICommandProxy(), null);
            controller.InvokeStartOrder(TransactionType.Buy, "STOCK01");

            Assert.AreEqual("STOCK01", presentationModel.TransactionInfo.TickerSymbol);
            Assert.AreEqual(TransactionType.Buy, presentationModel.TransactionInfo.TransactionType);
        }

        [TestMethod]
        public void SubmitAllInstanceCommandHookedToGlobalSubmitAllCommands()
        {
            var container = new MockUnityContainer();
            var orderCompositePresenter = new MockOrderCompositePresentationModel();
            container.ResolveBag.Add(typeof(IOrdersPresentationModel), new MockOrdersPresentationModel());
            container.ResolveBag.Add(typeof(IOrderCompositePresentationModel), orderCompositePresenter);
            var commandProxy = new MockStockTraderRICommandProxy();
            
            var accountPositionService = new MockAccountPositionService();
            accountPositionService.AddPosition("STOCK01", 10.0M, 100);

            var controller = new TestableOrdersController(regionManager, container, commandProxy, accountPositionService);
            controller.InvokeStartOrder(TransactionType.Buy, "STOCK01");

            Assert.IsFalse(controller.SubmitAllCommandCalled);
            commandProxy.SubmitAllOrdersCommand.CanExecute(null);
            Assert.IsTrue(controller.SubmitAllCommandCalled);
        }

        [TestMethod]
        public void CannotSellMoreSharesThanAreOwned()
        {
            var container = new MockUnityContainer();
            container.ResolveBag.Add(typeof(IOrdersPresentationModel), new MockOrdersPresentationModel());
            var accountPositionService = new MockAccountPositionService();
            accountPositionService.AddPosition("STOCK01", 10.0M, 100);

            var controller = new TestableOrdersController(regionManager, container, new MockStockTraderRICommandProxy(), accountPositionService);
            var buyOrder = new MockOrderCompositePresentationModel() { Shares = 100, };

            container.ResolveBag.Add(typeof(IOrderCompositePresentationModel), buyOrder);
            controller.InvokeStartOrder(TransactionType.Buy, "STOCK01");

            Assert.IsTrue(controller.SubmitAllVoteOnlyCommand.CanExecute(null));

            var sellOrder = new MockOrderCompositePresentationModel() { Shares = 200 };
			container.ResolveBag[typeof(IOrderCompositePresentationModel)] = sellOrder;
            controller.InvokeStartOrder(TransactionType.Sell, "STOCK01");

            //Should not be able to sell even though owned shares==100, buy==100 and sell==200
            Assert.IsFalse(controller.SubmitAllVoteOnlyCommand.CanExecute(null));
        }

        [TestMethod]
        public void CannotSellMoreSharesThanAreOwnedInDifferentOrders()
        {
            var container = new MockUnityContainer();
            container.ResolveBag.Add(typeof(IOrdersPresentationModel), new MockOrdersPresentationModel());
            var accountPositionService = new MockAccountPositionService();
            accountPositionService.AddPosition("STOCK01", 10.0M, 100);

            var controller = new TestableOrdersController(regionManager, container, new MockStockTraderRICommandProxy(), accountPositionService);
            var sellOrder1 = new MockOrderCompositePresentationModel() { Shares = 100 };

            container.ResolveBag.Add(typeof(IOrderCompositePresentationModel), sellOrder1);
            controller.InvokeStartOrder(TransactionType.Sell, "STOCK01");

            Assert.IsTrue(controller.SubmitAllVoteOnlyCommand.CanExecute(null));

            var sellOrder2 = new MockOrderCompositePresentationModel() { Shares = 100 };
			container.ResolveBag[typeof(IOrderCompositePresentationModel)] = sellOrder2;
            controller.InvokeStartOrder(TransactionType.Sell, "stock01");

            Assert.IsFalse(controller.SubmitAllVoteOnlyCommand.CanExecute(null));
        }

        [TestMethod]
        public void CannotSellMoreSharesThatAreNotOwned()
        {
            var container = new MockUnityContainer();
            container.ResolveBag.Add(typeof(IOrdersPresentationModel), new MockOrdersPresentationModel());

            var controller = new TestableOrdersController(regionManager, container, new MockStockTraderRICommandProxy(), new MockAccountPositionService());

            var sellOrder = new MockOrderCompositePresentationModel() { Shares = 1};
            container.ResolveBag.Add(typeof(IOrderCompositePresentationModel), sellOrder);

            controller.InvokeStartOrder(TransactionType.Sell, "NOTOWNED");

            Assert.IsFalse(controller.SubmitAllVoteOnlyCommand.CanExecute(null));
        }

        [TestMethod]
        public void CannotSubmitAllWhenNothingToSubmit()
        {
            var controller = new TestableOrdersController(new MockRegionManager(), new MockUnityContainer(), new MockStockTraderRICommandProxy(), new MockAccountPositionService());

            Assert.IsFalse(controller.SubmitAllVoteOnlyCommand.CanExecute(null));
        }

        [TestMethod]
        public void AfterAllOrdersSubmittedSubmitAllCommandShouldBeDisabled()
        {
            var container = new MockUnityContainer();
            container.ResolveBag.Add(typeof(IOrdersPresentationModel), new MockOrdersPresentationModel());
            var commandProxy = new MockStockTraderRICommandProxy();

            var controller = new TestableOrdersController(regionManager, container, commandProxy, new MockAccountPositionService());

            var buyOrder = new MockOrderCompositePresentationModel() { Shares = 100 };
			container.ResolveBag[typeof(IOrderCompositePresentationModel)] = buyOrder;
            controller.InvokeStartOrder(TransactionType.Buy, "STOCK1");

            bool canExecuteChangedCalled = false;
            bool canExecuteResult = false;

            commandProxy.SubmitAllOrdersCommand.CanExecuteChanged += delegate
                                                                 {
                                                                     canExecuteChangedCalled = true;
                                                                     canExecuteResult =
                                                                         controller.SubmitAllVoteOnlyCommand.CanExecute(null);
                                                                 };
            buyOrder.RaiseCloseViewRequested();

            Assert.IsTrue(canExecuteChangedCalled);
            Assert.IsFalse(canExecuteResult);
        }

        [TestMethod]
        public void ShouldRemoveOrdersViewWhenClosingLastOrder()
        {
            var container = new MockUnityContainer();
            container.ResolveBag.Add(typeof(IOrdersPresentationModel), new MockOrdersPresentationModel());
            var region = (MockRegion)regionManager.Regions["ActionRegion"];

            var controller = new TestableOrdersController(regionManager, container, new MockStockTraderRICommandProxy(), null);

            var buyOrder = new MockOrderCompositePresentationModel() { Shares = 100 };
			container.ResolveBag[typeof(IOrderCompositePresentationModel)] = buyOrder;
            controller.InvokeStartOrder(TransactionType.Buy, "STOCK1");

            Assert.AreEqual<int>(1, region.AddedViews.Count);

            buyOrder.RaiseCloseViewRequested();

            Assert.AreEqual<int>(0, region.AddedViews.Count);
        }
    }

    internal class TestableOrdersController : OrdersController
    {

        public TestableOrdersController(IRegionManager regionManager, IUnityContainer container, MockStockTraderRICommandProxy commandProxy, IAccountPositionService accountPositionService)
            : base(regionManager, container, commandProxy, accountPositionService)
        {
        }

        public string StartOrderArgumentTickerSymbol { get; set; }
        public TransactionType StartOrderArgumentTransactionType { get; set; }

        

        protected override void StartOrder(string tickerSymbol, TransactionType transactionType)
        {
            base.StartOrder(tickerSymbol, transactionType);

            StartOrderArgumentTickerSymbol = tickerSymbol;
            StartOrderArgumentTransactionType = transactionType;
        }

        public void InvokeStartOrder(TransactionType transactionType, string symbol)
        {
            StartOrder(symbol, transactionType);
        }

        public bool SubmitAllCommandCalled = false;

        protected override bool SubmitAllCanExecute(object parameter)
        {
            SubmitAllCommandCalled = true;
            return base.SubmitAllCanExecute(parameter);
        }
    }

    public class MockOrdersPresentationModel : IOrdersPresentationModel
    {
        private IOrdersView _view = new MockOrdersView();

        public IOrdersView View
        {
            get { return _view; }
            set { _view = value; }
        }

        public string HeaderInfo
        {
            get { throw new NotImplementedException(); }
        }
    }

    class MockOrderCompositePresentationModel : IOrderCompositePresentationModel
    {
        public MockCommand MockSubmitCommand = new MockCommand();
        public MockCommand MockCancelCommand = new MockCommand();
        private IOrderCompositeView _view = new MockOrderCompositeView();

        public event EventHandler CloseViewRequested;

        public IOrderCompositeView View
        {
            get { return _view; }
        }

        public ICommand SubmitCommand
        {
            get { return MockSubmitCommand; }
        }

        public ICommand CancelCommand
        {
            get { return MockCancelCommand; }
        }

        public TransactionInfo TransactionInfo { get; set;}

        public int Shares { get; set;}

        internal void RaiseCloseViewRequested()
        {
            CloseViewRequested(this, EventArgs.Empty);
        }
    }

    internal class MockCommand : ICommand
    {
        public bool ExecuteCalled;
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            ExecuteCalled = true;
        }
    }
}

