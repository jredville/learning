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
namespace ModularityWithMef.Silverlight
{
    using System;
    using System.Windows;
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;
    using Microsoft.Practices.Composite.Logging;
    using Microsoft.Practices.Composite.Modularity;
    using Microsoft.Practices.Composite.MefExtensions;

    /// <summary>
    /// Initializes Prism to start this quickstart Prism application to use Managed Extensibility Framework (MEF).
    /// </summary>
    public class Bootstrapper : MefBootstrapper
    {
        private readonly CallbackLogger callbackLogger = new CallbackLogger();

        /// <summary>
        /// Creates the shell or main window of the application.
        /// </summary>
        /// <returns>The shell of the application.</returns>
        /// <remarks>
        /// If the returned instance is a <see cref="DependencyObject"/>, the
        /// <see cref="MefBootstrapper"/> will attach the default <seealso cref="IRegionManager"/> of
        /// the application in its <see cref="RegionManager.RegionManagerProperty"/> attached property
        /// in order to be able to add regions by using the <seealso cref="RegionManager.RegionNameProperty"/>
        /// attached property from XAML.
        /// </remarks>
        protected override DependencyObject CreateShell()
        {
            Shell mainPage = new Shell();
            Application.Current.RootVisual = mainPage;
            return mainPage;
        }

        /// <summary>
        /// Configures the <see cref="AggregateCatalog"/> used by MEF.
        /// </summary>
        /// <remarks>
        /// The base implementation does nothing.
        /// </remarks>
        protected override void ConfigureAggregateCatalog()
        {
            base.ConfigureAggregateCatalog();

            // Add this assembly to export ModuleTracker
            this.AggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof(Bootstrapper).Assembly));

            // Module A is referenced in in the project and directly in code.
            this.AggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof(ModuleA.ModuleA).Assembly));

            // Module C is referenced in in the project and directly in code.
            this.AggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof(ModuleC.ModuleC).Assembly));            
        }

        /// <summary>
        /// Configures the <see cref="CompositionContainer"/>.
        /// May be overwritten in a derived class to add specific type mappings required by the application.
        /// </summary>
        /// <remarks>
        /// The base implementation registers all the types direct instantiated by the bootstrapper with the container.
        /// The base implementation also sets the ServiceLocator provider singleton.
        /// </remarks>
        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();

            // Because we created the CallbackLogger and it needs to be used immediately, we compose it to satisfy any imports it has.
            this.Container.ComposeExportedValue<CallbackLogger>(this.callbackLogger);
        }

        /// <summary>
        /// Creates the <see cref="IModuleCatalog"/> used by Prism.
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// The base implementation returns a new ModuleCatalog.
        /// </remarks>
        protected override IModuleCatalog CreateModuleCatalog()
        {
            // When using MEF, the existing Prism ModuleCatalog is still the place to configure modules via configuration files.
            return Microsoft.Practices.Composite.Modularity.ModuleCatalog.CreateFromXaml(new Uri("/ModularityWithMef.Silverlight;component/ModulesCatalog.xaml", UriKind.Relative));
        }

        /// <summary>
        /// Create the <see cref="ILoggerFacade"/> used by the bootstrapper.
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// The base implementation returns a new TextLogger.
        /// </remarks>
        protected override ILoggerFacade CreateLogger()
        {
            return this.callbackLogger;
        }
    }
}


