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
using System.ComponentModel;
using System.Globalization;
using Microsoft.Practices.Composite.Presentation.Commands;
using StockTraderRI.Infrastructure;
using StockTraderRI.Infrastructure.Interfaces;
using StockTraderRI.Infrastructure.Models;
using StockTraderRI.Modules.Position.Interfaces;
using StockTraderRI.Modules.Position.Models;
using StockTraderRI.Modules.Position.Properties;

namespace StockTraderRI.Modules.Position.Orders
{
    public class OrderDetailsPresentationModel : IOrderDetailsPresentationModel, INotifyPropertyChanged
    {
        private readonly IAccountPositionService accountPositionService;
        private readonly IOrdersService ordersService;
        private TransactionInfo transactionInfo;
        private int? shares;
        private OrderType orderType = OrderType.Market;
        private decimal? stopLimitPrice;
        private TimeInForce timeInForce;

        private readonly List<string> errors = new List<string>();

        public OrderDetailsPresentationModel(IOrderDetailsView view, IAccountPositionService accountPositionService, IOrdersService ordersService)
        {
            this.View = view;
            this.accountPositionService = accountPositionService;
            this.ordersService = ordersService;

            this.transactionInfo = new TransactionInfo();

            //use localizable enum descriptions
            this.AvailableOrderTypes = new List<ValueDescription<OrderType>>
                                        {
                                            new ValueDescription<OrderType>(OrderType.Limit, Resources.OrderType_Limit),
                                            new ValueDescription<OrderType>(OrderType.Market, Resources.OrderType_Market),
                                            new ValueDescription<OrderType>(OrderType.Stop, Resources.OrderType_Stop)
                                        };

            this.AvailableTimesInForce = new List<ValueDescription<TimeInForce>>
                                          {
                                              new ValueDescription<TimeInForce>(TimeInForce.EndOfDay, Resources.TimeInForce_EndOfDay),
                                              new ValueDescription<TimeInForce>(TimeInForce.ThirtyDays, Resources.TimeInForce_ThirtyDays)
                                          };

            this.SubmitCommand = new DelegateCommand<object>(this.Submit, this.CanSubmit);
            this.CancelCommand = new DelegateCommand<object>(this.Cancel);

            this.View.SetModel(this);
            this.SetInitialValidState();
        }

        public event EventHandler CloseViewRequested = delegate { };

        public IOrderDetailsView View { get; set; }

        public IList<ValueDescription<OrderType>> AvailableOrderTypes { get; private set; }

        public IList<ValueDescription<TimeInForce>> AvailableTimesInForce { get; private set; }

        public TransactionInfo TransactionInfo
        {
            get { return this.transactionInfo; }
            set
            {
                this.transactionInfo = value;
                OnPropertyChanged("TransactionType");
                OnPropertyChanged("TickerSymbol");
            }
        }

        public TransactionType TransactionType
        {
            get { return this.transactionInfo.TransactionType; }
            set
            {
                this.ValidateHasEnoughSharesToSell(this.Shares, value, false);

                if (this.transactionInfo.TransactionType != value)
                {
                    this.transactionInfo.TransactionType = value;
                    OnPropertyChanged("TransactionType");
                }
            }
        }

        public string TickerSymbol
        {
            get { return this.transactionInfo.TickerSymbol; }
            set
            {
                if (this.transactionInfo.TickerSymbol != value)
                {
                    this.transactionInfo.TickerSymbol = value;
                    OnPropertyChanged("TickerSymbol");
                }
            }
        }

        public int? Shares
        {
            get { return this.shares; }
            set
            {
                this.ValidateShares(value, true);
                this.ValidateHasEnoughSharesToSell(value, this.TransactionType, true);

                if (this.shares != value)
                {
                    this.shares = value;
                    OnPropertyChanged("Shares");
                }
            }
        }

        public OrderType OrderType
        {
            get { return this.orderType; }
            set
            {
                if (!value.Equals(this.orderType))
                {
                    this.orderType = value;
                    OnPropertyChanged("OrderType");
                }
            }
        }

        public decimal? StopLimitPrice
        {
            get
            {
                return this.stopLimitPrice;
            }
            set
            {
                this.ValidateStopLimitPrice(value, true);

                if (value != this.stopLimitPrice)
                {
                    this.stopLimitPrice = value;
                    OnPropertyChanged("StopLimitPrice");
                }
            }
        }

        public TimeInForce TimeInForce
        {
            get { return this.timeInForce; }
            set
            {
                if (value != this.timeInForce)
                {
                    this.timeInForce = value;
                    OnPropertyChanged("TimeInForce");
                }

                this.timeInForce = value;
            }
        }

        public DelegateCommand<object> SubmitCommand { get; private set; }

        public DelegateCommand<object> CancelCommand { get; private set; }

        private void SetInitialValidState()
        {
            this.ValidateShares(this.Shares, false);
            this.ValidateStopLimitPrice(this.StopLimitPrice, false);
        }

        private void ValidateShares(int? newSharesValue, bool throwException)
        {
            if (!newSharesValue.HasValue || newSharesValue.Value <= 0)
            {
                this.AddError("InvalidSharesRange");
                if (throwException)
                {
                    throw new InputValidationException(Resources.InvalidSharesRange);
                }
            }
            else
            {
                this.RemoveError("InvalidSharesRange");
            }
        }

        private void ValidateStopLimitPrice(decimal? price, bool throwException)
        {
            if (!price.HasValue || price.Value <= 0)
            {
                this.AddError("InvalidStopLimitPrice");
                if (throwException)
                {
                    throw new InputValidationException(Resources.InvalidStopLimitPrice);
                }
            }
            else
            {
                this.RemoveError("InvalidStopLimitPrice");
            }
        }

        private void ValidateHasEnoughSharesToSell(int? sharesToSell, TransactionType transactionType, bool throwException)
        {
            if (transactionType == TransactionType.Sell && !this.HoldsEnoughShares(this.TickerSymbol, sharesToSell))
            {
                this.AddError("NotEnoughSharesToSell");
                if (throwException)
                {
                    throw new InputValidationException(String.Format(CultureInfo.InvariantCulture, Resources.NotEnoughSharesToSell, sharesToSell));
                }
            }
            else
            {
                this.RemoveError("NotEnoughSharesToSell");
            }
        }

        private void AddError(string ruleName)
        {
            if (!this.errors.Contains(ruleName))
            {
                this.errors.Add(ruleName);
                this.SubmitCommand.RaiseCanExecuteChanged();
            }
        }

        private void RemoveError(string ruleName)
        {
            if (this.errors.Contains(ruleName))
            {
                this.errors.Remove(ruleName);
                if (this.errors.Count == 0)
                {
                    this.SubmitCommand.RaiseCanExecuteChanged();
                }
            }
        }

        private bool CanSubmit(object parameter)
        {
            return this.errors.Count == 0;
        }

        private bool HoldsEnoughShares(string symbol, int? sharesToSell)
        {
            if (!sharesToSell.HasValue)
            {
                return false;
            }

            foreach (AccountPosition accountPosition in this.accountPositionService.GetAccountPositions())
            {
                if (accountPosition.TickerSymbol.Equals(symbol, StringComparison.OrdinalIgnoreCase))
                {
                    if (accountPosition.Shares >= sharesToSell)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            return false;
        }

        private void Submit(object parameter)
        {
            if (!this.CanSubmit(parameter))
            {
                throw new InvalidOperationException();
            }

            var order = new Order();
            order.TransactionType = this.TransactionType;
            order.OrderType = this.OrderType;
            order.Shares = this.Shares.Value;
            order.StopLimitPrice = this.StopLimitPrice.Value;
            order.TickerSymbol = this.TickerSymbol;
            order.TimeInForce = this.TimeInForce;

            ordersService.Submit(order);

            CloseViewRequested(this, EventArgs.Empty);
        }

        private void Cancel(object parameter)
        {
            CloseViewRequested(this, EventArgs.Empty);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
