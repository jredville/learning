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
using System.ComponentModel;
using Microsoft.Practices.Composite.Events;
using Microsoft.Practices.Composite.Presentation.Events;
using StockTraderRI.Infrastructure;
using StockTraderRI.Modules.Position.Interfaces;
using System.Windows.Input;
using StockTraderRI.Modules.Position.Controllers;

namespace StockTraderRI.Modules.Position.PositionSummary
{
    public class PositionSummaryPresentationModel : IPositionSummaryPresentationModel, INotifyPropertyChanged
    {
        private PositionSummaryItem currentPositionSummaryItem;

        private readonly IEventAggregator eventAggregator;

        public event PropertyChangedEventHandler PropertyChanged;

        public IPositionSummaryView View { get; set; }

        public IObservablePosition Position { get; private set; }

        public PositionSummaryPresentationModel(IPositionSummaryView view, IOrdersController ordersController, IEventAggregator eventAggregator, IObservablePosition observablePosition)
        {
            this.eventAggregator = eventAggregator;
            this.Position = observablePosition;

            View = view;

            BuyCommand = ordersController.BuyCommand;
            SellCommand = ordersController.SellCommand;

            View.Model = this;

            this.CurrentPositionSummaryItem = new PositionSummaryItem("FAKEINDEX", 0, 0, 0);
        }

        public ICommand BuyCommand { get; private set; }

        public ICommand SellCommand { get; private set; }

        public string HeaderInfo
        {
            get { return "POSITION"; }
        }

        public PositionSummaryItem CurrentPositionSummaryItem
        {
            get { return currentPositionSummaryItem; }
            set
            {
                if (currentPositionSummaryItem != value)
                {
                    currentPositionSummaryItem = value;
                    InvokePropertyChanged(new PropertyChangedEventArgs("CurrentPositionSummaryItem"));
                    if (currentPositionSummaryItem != null)
                    {
                        eventAggregator.GetEvent<TickerSymbolSelectedEvent>().Publish(
                            CurrentPositionSummaryItem.TickerSymbol);
                    }
                }
            }
        }

        private void InvokePropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChangedEventHandler Handler = PropertyChanged;
            if (Handler != null)
            {
                Handler(this, e);
            }
        }
    }
}
