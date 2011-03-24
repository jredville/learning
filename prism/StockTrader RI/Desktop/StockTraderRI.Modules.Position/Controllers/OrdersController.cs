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
using System.Globalization;
using Microsoft.Practices.Composite.Presentation.Commands;
using Microsoft.Practices.Composite.Regions;
using Microsoft.Practices.Unity;
using StockTraderRI.Infrastructure;
using StockTraderRI.Infrastructure.Interfaces;
using StockTraderRI.Infrastructure.Models;
using StockTraderRI.Modules.Position.Interfaces;
using StockTraderRI.Modules.Position.Models;
using StockTraderRI.Modules.Position.Orders;
using StockTraderRI.Modules.Position.Properties;
using System.Linq;

namespace StockTraderRI.Modules.Position.Controllers
{
    public class OrdersController : IOrdersController
    {
        private IRegionManager _regionManager;
        private IUnityContainer _container;
        private readonly StockTraderRICommandProxy commandProxy;
        private IAccountPositionService _accountPositionService;

        public OrdersController(IRegionManager regionManager, IUnityContainer container, StockTraderRICommandProxy commandProxy, IAccountPositionService accountPositionService)
        {
            _regionManager = regionManager;
            _container = container;
            _accountPositionService = accountPositionService;
            this.commandProxy = commandProxy;
            BuyCommand = new DelegateCommand<string>(OnBuyExecuted);
            SellCommand = new DelegateCommand<string>(OnSellExecuted);
            SubmitAllVoteOnlyCommand = new DelegateCommand<object>(null, SubmitAllCanExecute);
            OrderModels = new List<IOrderCompositePresentationModel>();
            commandProxy.SubmitAllOrdersCommand.RegisterCommand(SubmitAllVoteOnlyCommand);
            
        }

        void OnSellExecuted(string parameter)
        {
            StartOrder(parameter, TransactionType.Sell);
        }

        void OnBuyExecuted(string parameter)
        {
            StartOrder(parameter, TransactionType.Buy);
        }

        virtual protected bool SubmitAllCanExecute(object parameter)
        {
            Dictionary<string,long> sellOrderShares = new Dictionary<string, long>();

            if (OrderModels.Count == 0) return false;

            foreach(var order in OrderModels)
            {
                if (order.TransactionInfo.TransactionType == TransactionType.Sell)
                {
                    string tickerSymbol = order.TransactionInfo.TickerSymbol.ToUpper(CultureInfo.CurrentCulture);
                    if (!sellOrderShares.ContainsKey(tickerSymbol))
                        sellOrderShares.Add(tickerSymbol, 0);

                    //populate dictionary with total shares bought or sold by tickersymbol
                    sellOrderShares[tickerSymbol] += order.Shares;
                }
            }

            IList<AccountPosition> positions = _accountPositionService.GetAccountPositions();

            foreach (string key in sellOrderShares.Keys)
            {
                AccountPosition position =
                    positions.FirstOrDefault(
                        x => String.Compare(x.TickerSymbol, key, StringComparison.CurrentCultureIgnoreCase) == 0);
                if (position == null || position.Shares < sellOrderShares[key])
                {
                    //trying to sell more shares than we own
                    return false;
                }
            }

            return true;
            
        }

        virtual protected void StartOrder(string tickerSymbol, TransactionType transactionType)
        {
            if (String.IsNullOrEmpty(tickerSymbol))
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Resources.StringCannotBeNullOrEmpty, "tickerSymbol"));
            }
            this.ShowOrdersView();

            IRegion ordersRegion = _regionManager.Regions[RegionNames.OrdersRegion];

            var orderCompositePresentationModel = _container.Resolve<IOrderCompositePresentationModel>();
            orderCompositePresentationModel.TransactionInfo = new TransactionInfo(tickerSymbol, transactionType);
            orderCompositePresentationModel.CloseViewRequested += delegate
            {
                OrderModels.Remove(orderCompositePresentationModel);
                commandProxy.SubmitAllOrdersCommand.UnregisterCommand(orderCompositePresentationModel.SubmitCommand);
                commandProxy.CancelAllOrdersCommand.UnregisterCommand(orderCompositePresentationModel.CancelCommand);
                commandProxy.SubmitOrderCommand.UnregisterCommand(orderCompositePresentationModel.SubmitCommand);
                commandProxy.CancelOrderCommand.UnregisterCommand(orderCompositePresentationModel.CancelCommand);
                ordersRegion.Remove(orderCompositePresentationModel.View);
                if (ordersRegion.Views.Count() == 0)
                {
                    this.RemoveOrdersView();
                }
            };

            ordersRegion.Add(orderCompositePresentationModel.View);
            OrderModels.Add(orderCompositePresentationModel);

            commandProxy.SubmitAllOrdersCommand.RegisterCommand(orderCompositePresentationModel.SubmitCommand);
            commandProxy.CancelAllOrdersCommand.RegisterCommand(orderCompositePresentationModel.CancelCommand);
            commandProxy.SubmitOrderCommand.RegisterCommand(orderCompositePresentationModel.SubmitCommand);
            commandProxy.CancelOrderCommand.RegisterCommand(orderCompositePresentationModel.CancelCommand);

            ordersRegion.Activate(orderCompositePresentationModel.View);
        }

        private void RemoveOrdersView()
        {
            IRegion region = this._regionManager.Regions[RegionNames.ActionRegion];

            object ordersView = region.GetView("OrdersView");
            if (ordersView != null)
            {
                region.Remove(ordersView);
            }
        }

        private void ShowOrdersView()
        {
            IRegion region = this._regionManager.Regions[RegionNames.ActionRegion];

            object ordersView = region.GetView("OrdersView");
            if (ordersView == null)
            {
                var ordersPresentationModel = this._container.Resolve<IOrdersPresentationModel>();
                ordersView = ordersPresentationModel.View;
                region.Add(ordersView, "OrdersView");
            }

            region.Activate(ordersView);
        }

        #region IOrdersController Members

        public DelegateCommand<string> BuyCommand { get; private set; }
        public DelegateCommand<string> SellCommand { get; private set; }
        public DelegateCommand<object> SubmitAllVoteOnlyCommand{ get; private set; }

        private List<IOrderCompositePresentationModel> OrderModels { get; set;}

        #endregion
    }
}
