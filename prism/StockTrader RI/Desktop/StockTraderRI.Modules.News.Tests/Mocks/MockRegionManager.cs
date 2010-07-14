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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.Practices.Composite.Regions;

namespace StockTraderRI.Modules.News.Tests.Mocks
{
    public class MockRegionManager : IRegionManager
    {
        private MockRegionCollection _regions = new MockRegionCollection();

        public IRegionCollection Regions
        {
            get { return _regions; }
        }

        public IRegion AttachNewRegion(object regionTarget, string regionName)
        {
            throw new NotImplementedException();
        }

        public IRegionManager CreateRegionManager()
        {
            throw new NotImplementedException();
        }
    }

    internal class MockRegionCollection : IRegionCollection
    {
        private Dictionary<string, IRegion> regions =  new Dictionary<string, IRegion>();

        public IEnumerator<IRegion> GetEnumerator()
        {
            throw new System.NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IRegion this[string regionName]
        {
            get { return regions[regionName]; }
        }

        public void Add(IRegion region)
        {
            regions[region.Name] = region;
        }

        public bool Remove(string regionName)
        {
            throw new System.NotImplementedException();
        }

        public bool ContainsRegionWithName(string regionName)
        {
            throw new System.NotImplementedException();
        }
    }

    public class MockNewsRegion : IRegion
    {
        public object GetViewReturnValue;
        public string GetViewArgumentName;
        public object ShowArgumentView;
        public string AddArgumentName;
        public IList<object> AddedViews = new List<object>();

        #region IRegion Members

        public string Name { get; set; }

        public IRegionManager Add(object view)
        {
            AddedViews.Add(view);
            return null;
        }

        public void Remove(object view)
        {
            throw new NotImplementedException();
        }

        public IViewsCollection Views
        {
            get { throw new NotImplementedException(); }
        }

        public void Activate(object view)
        {
            ShowArgumentView = view;
        }

        public void Deactivate(object view)
        {
            throw new NotImplementedException();
        }

        public IRegionManager Add(object view, string viewName)
        {
            AddArgumentName = viewName;
            Add(view);
            return null;
        }

        public object GetView(string viewName)
        {
            GetViewArgumentName = viewName;
            return GetViewReturnValue;
        }

        public IRegionManager Add(object view, string viewName, bool createRegionManagerScope)
        {
            throw new NotImplementedException();
        }

        public IRegionManager RegionManager
        {
            set { throw new NotImplementedException(); }
            get { throw new NotImplementedException();}
        }

        public IRegionBehaviorCollection Behaviors
        {
            get { throw new System.NotImplementedException(); }
        }

        public IViewsCollection ActiveViews
        {
            get { throw new NotImplementedException(); }
        }

        public object Context
        {
            get { throw new System.NotImplementedException(); }
            set { throw new System.NotImplementedException(); }
        }

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
    }
}