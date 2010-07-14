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
using StockTraderRI.Modules.Watch.AddWatch;
using StockTraderRI.Modules.Watch.Controllers;
using StockTraderRI.Modules.Watch.Services;
using StockTraderRI.Modules.Watch.WatchList;

namespace StockTraderRI.Modules.Watch
{
    public class WatchModule : IModule
    {
        private readonly IUnityContainer _container;
        private readonly IRegionManager _regionManager;

        public WatchModule(IUnityContainer container, IRegionManager regionManager)
        {
            _container = container;
            _regionManager = regionManager;
        }

        #region IModule Members

        public void Initialize()
        {
            this.RegisterViewsAndServices();

            var controller = this._container.Resolve<IWatchListController>();
            controller.Run();

            this._regionManager.RegisterViewWithRegion(
                                                    RegionNames.MainToolBarRegion,
                                                    () => this._container.Resolve<IAddWatchPresenter>().View);
        }

        protected void RegisterViewsAndServices()
        {
            _container.RegisterType<IWatchListController, WatchListController>();
            _container.RegisterType<IWatchListService, WatchListService>(new ContainerControlledLifetimeManager());

            _container.RegisterType<IWatchListView, WatchListView>();
            _container.RegisterType<IWatchListPresentationModel, WatchListPresentationModel>();

            _container.RegisterType<IAddWatchView, AddWatchView>();
            _container.RegisterType<IAddWatchPresenter, AddWatchPresenter>();
        }

        #endregion
    }
}
