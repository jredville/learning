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
using System.Windows;
using Microsoft.Practices.Composite.Modularity;
using Microsoft.Practices.Composite.UnityExtensions;
using Microsoft.Practices.Unity;
using StockTraderRI.Modules.Market;
using StockTraderRI.Modules.News;
using StockTraderRI.Modules.Position;
using StockTraderRI.Modules.Watch;

namespace StockTraderRI
{
    public partial class StockTraderRIBootstrapper : UnityBootstrapper
    {
        protected override IModuleCatalog CreateModuleCatalog()
        {
            var catalog = new ModuleCatalog();
            catalog.AddModule(typeof(MarketModule))
                .AddModule(typeof(PositionModule), "MarketModule")
                .AddModule(typeof(WatchModule), "MarketModule")
                .AddModule(typeof(NewsModule));

            return catalog;
        }

        protected override void ConfigureContainer()
        {
            Container.RegisterType<IShellView, Shell>();
            base.ConfigureContainer();
        }

        protected override DependencyObject CreateShell()
        {
			ShellPresenter presenter = Container.Resolve<ShellPresenter>();
			IShellView view = presenter.View;

			view.ShowView();

			return view as DependencyObject;
        }
    }
}
