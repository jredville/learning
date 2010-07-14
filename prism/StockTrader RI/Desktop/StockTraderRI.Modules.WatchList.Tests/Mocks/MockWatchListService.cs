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
using System.Collections.ObjectModel;
using System.Windows.Input;
using Microsoft.Practices.Composite.Presentation.Commands;
using StockTraderRI.Modules.Watch.Services;

namespace StockTraderRI.Modules.WatchList.Tests.Mocks
{
    class MockWatchListService : IWatchListService
    {
        internal ObservableCollection<string> MockWatchList = new ObservableCollection<string>();

        #region IWatchListService Members

        public ObservableCollection<string> RetrieveWatchList()
        {
            return MockWatchList;
        }

        private DelegateCommand<string> _testDelegate = new DelegateCommand<string>(delegate { });
        public ICommand AddWatchCommand
        {
            get
            {
                return _testDelegate;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        #endregion
    }
}
