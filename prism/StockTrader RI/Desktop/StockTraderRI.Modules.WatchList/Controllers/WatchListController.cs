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
using System.Collections.Specialized;
using Microsoft.Practices.Composite.Regions;
using StockTraderRI.Infrastructure;
using StockTraderRI.Modules.Watch.WatchList;

namespace StockTraderRI.Modules.Watch.Controllers
{
    public class WatchListController : IWatchListController
    {
        private readonly IRegionManager regionManager;
        private readonly IWatchListPresentationModel watchListPresentationModel;

        public WatchListController(IRegionManager regionManager, IWatchListPresentationModel watchListPresentationModel)
        {
            this.regionManager = regionManager;
            this.watchListPresentationModel = watchListPresentationModel;
        }

        public void Run()
        {
            this.regionManager.RegisterViewWithRegion(
                                                    RegionNames.MainRegion,
                                                    () => this.watchListPresentationModel.View);
            this.watchListPresentationModel.WatchListItems.CollectionChanged += this.WatchController_CollectionChanged;
        }

        private void WatchController_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                var mainRegion = this.regionManager.Regions[RegionNames.MainRegion];
                if (mainRegion != null)
                {
                    mainRegion.Activate(this.watchListPresentationModel.View);
                }
            }
        }
    }
}
