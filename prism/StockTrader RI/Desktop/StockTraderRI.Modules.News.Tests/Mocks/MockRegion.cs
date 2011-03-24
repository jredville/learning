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
using Microsoft.Practices.Composite.Regions;

namespace StockTraderRI.Modules.News.Tests.Mocks
{
    internal class MockRegion : IRegion
    {
        public string RequestedViewName;
        public bool ActivateCalled;
        public object ActivateArg;

        public string Name { get; set; }

        public object GetView(string viewName)
        {
            RequestedViewName = viewName;
            return null;
        }

        public void Activate(object view)
        {
            ActivateCalled = true;
            ActivateArg = view;
        }

        public IRegionManager Add(object view, string viewName)
        {
            return null;
        }

        #region Not Implemented members

        public event PropertyChangedEventHandler PropertyChanged;

        public IViewsCollection Views
        {
            get { throw new System.NotImplementedException(); }
        }

        public IViewsCollection ActiveViews
        {
            get { throw new System.NotImplementedException(); }
        }

        public object Context
        {
            get { throw new System.NotImplementedException(); }
            set { throw new System.NotImplementedException(); }
        }

        public IRegionManager Add(object view)
        {
            return null;
        }

        public IRegionManager Add(object view, string viewName, bool createRegionManagerScope)
        {
            throw new System.NotImplementedException();
        }

        public void Remove(object view)
        {
            throw new System.NotImplementedException();
        }

        public void Deactivate(object view)
        {
            throw new System.NotImplementedException();
        }

        public IRegionManager RegionManager
        {
            get { throw new System.NotImplementedException(); }
            set { throw new System.NotImplementedException(); }
        }

        public IRegionBehaviorCollection Behaviors
        {
            get { throw new System.NotImplementedException(); }
        }

        #endregion
    }
}