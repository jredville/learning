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
using System.Windows.Controls;
using StockTraderRI.Infrastructure;
using StockTraderRI.Modules.Position.Interfaces;

namespace StockTraderRI.Modules.Position.PositionSummary
{
    public partial class PositionSummaryView : UserControl, IPositionSummaryView
    {
        public PositionSummaryView()
        {
            InitializeComponent();
        }

        public IPositionSummaryPresentationModel Model
        {
            get
            {
                return this.DataContext as IPositionSummaryPresentationModel;
            }

            set
            {
                // Because in Silverlight you cannot bind to a RelativeSource, we are using Resources with an observable value,
                // in order to be able to bind to the Buy and Sell commands. 
                // The resources are declared in the XAML, because Silverlight has StaticResource markup only, so these
                // resources should be available when the control is initializing, even though the Value is yet not set.
                ((ObservableCommand) this.Resources["BuyCommand"]).Value = value != null ? value.BuyCommand : null;
                ((ObservableCommand) this.Resources["SellCommand"]).Value = value != null ? value.SellCommand : null;
                DataContext = value;
            }
        }
    }
}