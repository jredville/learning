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
using StockTraderRI.Modules.Position.Controllers;
using Microsoft.Practices.Composite.Presentation.Commands;

namespace StockTraderRI.Modules.Position.Tests.Mocks
{
    public class MockOrdersController : IOrdersController
    {
        #region IOrdersController Members
        DelegateCommand<string> _buyCommand = new DelegateCommand<string>(delegate { });
        public DelegateCommand<string> BuyCommand
        {
            get { return _buyCommand; }
        }

        DelegateCommand<string> _sellCommand = new DelegateCommand<string>(delegate { });
        public DelegateCommand<string> SellCommand
        {
            get { return _sellCommand; }
        }
        #endregion
    }
}
