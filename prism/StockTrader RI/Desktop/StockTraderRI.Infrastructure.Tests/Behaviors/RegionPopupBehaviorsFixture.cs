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
using System.Windows;
using Microsoft.Practices.Composite.Presentation.Regions;
using Microsoft.Practices.Composite.Regions;
using Microsoft.Practices.ServiceLocation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StockTraderRI.Infrastructure.Behaviors;

namespace StockTraderRI.Infrastructure.Tests.Behaviors
{
    [TestClass]
    public class RegionPopupBehaviorsFixture
    {
        [TestMethod]
        public void ShouldCreateRegion()
        {
            try
            {
                var regionManager = new MockRegionManager();
                ServiceLocator.SetLocatorProvider(() => new MockServiceLocator(() => regionManager));

                FrameworkElement hostControl = new MockFrameworkElement();
                RegionPopupBehaviors.RegisterNewPopupRegion(hostControl, "MyPopupRegion");

                Assert.IsTrue(regionManager.MockRegions.Regions.ContainsKey("MyPopupRegion"));
                Assert.IsNotNull(regionManager.MockRegions.Regions["MyPopupRegion"]);
                Assert.IsInstanceOfType(regionManager.MockRegions.Regions["MyPopupRegion"], typeof(SingleActiveRegion));
                Assert.IsTrue(regionManager.MockRegions.Regions["MyPopupRegion"].Behaviors.ContainsKey(DialogActivationBehavior.BehaviorKey));
#if SILVERLIGHT
                Assert.IsInstanceOfType(regionManager.MockRegions.Regions["MyPopupRegion"].Behaviors[DialogActivationBehavior.BehaviorKey], typeof(PopupDialogActivationBehavior));
#else
                Assert.IsInstanceOfType(regionManager.MockRegions.Regions["MyPopupRegion"].Behaviors[DialogActivationBehavior.BehaviorKey], typeof(WindowDialogActivationBehavior));
#endif
            }
            finally
            {
                ServiceLocator.SetLocatorProvider(() => null);
            }
        }

        internal class MockFrameworkElement : FrameworkElement
        {
        }

        internal class MockRegionManager : IRegionManager
        {
            public MockRegions MockRegions = new MockRegions();

            public IRegionCollection Regions { get { return MockRegions; } }

            #region Not implemented members

            public IRegionManager CreateRegionManager()
            {
                throw new System.NotImplementedException();
            }

            #endregion
        }

        internal class MockRegions : IRegionCollection
        {
            public Dictionary<string, IRegion> Regions = new Dictionary<string, IRegion>();

            public IRegion this[string regionName]
            {
                get { return this.Regions[regionName]; }
            }

            public void Add(IRegion region)
            {
                this.Regions.Add(region.Name, region);
            }

            #region Not implemented members

            public IEnumerator<IRegion> GetEnumerator()
            {
                throw new NotImplementedException();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                throw new NotImplementedException();
            }

            public bool Remove(string regionName)
            {
                throw new NotImplementedException();
            }

            public bool ContainsRegionWithName(string regionName)
            {
                throw new NotImplementedException();
            }

            #endregion
        }

        internal class MockServiceLocator : ServiceLocatorImplBase
        {
            public Func<object> ResolveMethod;

            public MockServiceLocator(Func<object> resolveMethod)
            {
                this.ResolveMethod = resolveMethod;
            }

            protected override object DoGetInstance(Type serviceType, string key)
            {
                return this.ResolveMethod();
            }

            #region Not implemented members

            protected override IEnumerable<object> DoGetAllInstances(Type serviceType)
            {
                throw new NotImplementedException();
            }

            #endregion
        }
    }
}
