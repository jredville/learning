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

namespace StockTraderRI.Modules.Position.Tests.Mocks
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
        private Dictionary<string, IRegion> regions = new Dictionary<string, IRegion>();

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

    public class MockRegion : IRegion
    {
        public List<object> AddedViews = new List<object>();

        public string Name { get; set; }

        public IRegionManager Add(object view)
        {
            AddedViews.Add(view);
            return null;
        }

        public void Remove(object view)
        {
            AddedViews.Remove(view);
        }

        public IViewsCollection Views
        {
            get { return new MockViewsCollection(AddedViews); }
        }

        public void Activate(object view)
        {
            SelectedItem = view;
        }

        public void Deactivate(object view)
        {
            throw new NotImplementedException();
        }

        public IRegionManager Add(object view, string viewName)
        {
            Add(view);
            return null;
        }

        public object GetView(string viewName)
        {
            return AddedViews.Count > 0 ? AddedViews[0] : null;
        }

        public IRegionManager Add(object view, string viewName, bool createRegionManagerScope)
        {
            throw new NotImplementedException();
        }

        public IRegionManager RegionManager
        {
            get { throw new NotImplementedException(); }         
            set { throw new NotImplementedException(); }
        }

        public IRegionBehaviorCollection Behaviors
        {
            get { throw new System.NotImplementedException(); }
        }

        public object SelectedItem { get; set; }

        public IViewsCollection ActiveViews
        {
            get { throw new NotImplementedException(); }
        }

        public object Context
        {
            get { throw new System.NotImplementedException(); }
            set { throw new System.NotImplementedException(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    internal class MockViewsCollection : IViewsCollection
    {
        private readonly IList<object> views;

        public MockViewsCollection(IList<object> views)
        {
            this.views = views;
        }

        public bool Contains(object value)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<object> GetEnumerator()
        {
            return views.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public event System.Collections.Specialized.NotifyCollectionChangedEventHandler CollectionChanged;
    }
}
