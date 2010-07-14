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
using Microsoft.Practices.Composite.Presentation.Commands;
using StockTraderRI.Infrastructure;

namespace StockTraderRI.Modules.Position.Tests.Mocks
{
    public class MockStockTraderRICommandProxy : StockTraderRICommandProxy
    {
        CompositeCommand _submitAllOrdersCommand = new CompositeCommand();
        CompositeCommand _cancelAllOrdersCommand = new CompositeCommand();
        CompositeCommand _submitOrderCommand = new CompositeCommand(true);
        CompositeCommand _cancelOrderCommand = new CompositeCommand(true);

        public override CompositeCommand SubmitOrderCommand
        {
            get
            {
                return this._submitOrderCommand;
            }
        }

        public override CompositeCommand SubmitAllOrdersCommand
        {
            get
            {
                return this._submitAllOrdersCommand;
            }
        }
        public override CompositeCommand CancelOrderCommand
        {
            get
            {
                return this._cancelOrderCommand;
            }
        }

        public override CompositeCommand CancelAllOrdersCommand
        {
            get
            {
                return this._cancelAllOrdersCommand;
            }
        }
    }
}
