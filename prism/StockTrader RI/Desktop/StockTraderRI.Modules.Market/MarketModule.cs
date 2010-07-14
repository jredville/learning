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
using Microsoft.Practices.Composite.Modularity;
using Microsoft.Practices.Composite.Regions;
using Microsoft.Practices.Unity;
using StockTraderRI.Infrastructure;
using StockTraderRI.Infrastructure.Interfaces;
using StockTraderRI.Modules.Market.Services;
using StockTraderRI.Modules.Market.TrendLine;

namespace StockTraderRI.Modules.Market
{
    public class MarketModule : IModule
    {
        private readonly IUnityContainer container;
        private readonly IRegionManager regionManager;

        public MarketModule(IUnityContainer container, IRegionManager regionManager)
        {
            this.container = container;
            this.regionManager = regionManager;
        }

        #region IModule Members

        public void Initialize()
        {
            RegisterViewsAndServices();

            this.regionManager.RegisterViewWithRegion(RegionNames.ResearchRegion, () => this.container.Resolve<ITrendLinePresentationModel>().View);
        }

        protected void RegisterViewsAndServices()
        {
            container.RegisterType<IMarketHistoryService, MarketHistoryService>(new ContainerControlledLifetimeManager());
            container.RegisterType<IMarketFeedService, MarketFeedService>(new ContainerControlledLifetimeManager());
            container.RegisterType<ITrendLineView, TrendLineView>();
            container.RegisterType<ITrendLinePresentationModel, TrendLinePresentationModel>();
        }

        #endregion
    }
}
