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
namespace ModularityWithUnity.Silverlight
{
    using System;
    using System.Windows;
    using Microsoft.Practices.Composite.Logging;
    using Microsoft.Practices.Composite.Modularity;
    using Microsoft.Practices.Unity;
    using Microsoft.Practices.Composite.UnityExtensions;
    using ModuleTracking;

    /// <summary>
    /// Initializes Prism to start this quickstart Prism application to use Unity.
    /// </summary>
    public class Bootstrapper : UnityBootstrapper
    {
        private readonly CallbackLogger callbackLogger = new CallbackLogger();

        /// <summary>
        /// Create the <see cref="ILoggerFacade"/> used by the bootstrapper.
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// The base implementation returns a new TextLogger.
        /// </remarks>
        protected override ILoggerFacade CreateLogger()
        {
            // Because the Shell is displayed after most of the interesting boostrapper work has been performed,
            // this quickstart uses a special logger class to hold on to early log entries and display them 
            // after the UI is visible.
            return this.callbackLogger;
        }

        /// <summary>
        /// Configures the <see cref="IUnityContainer"/>. May be overwritten in a derived class to add specific
        /// type mappings required by the application.
        /// </summary>
        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();

            this.RegisterTypeIfMissing(typeof(IModuleTracker), typeof(ModuleTracker), true);

            this.Container.RegisterInstance<CallbackLogger>(this.callbackLogger);
        }


        /// <summary>
        /// Creates the shell or main window of the application.
        /// </summary>
        /// <returns>The shell of the application.</returns>
        /// <remarks>
        /// If the returned instance is a <see cref="DependencyObject"/>, the
        /// <see cref="Bootstrapper"/> will attach the default <seealso cref="IRegionManager"/> of
        /// the application in its <see cref="RegionManager.RegionManagerProperty"/> attached property
        /// in order to be able to add regions by using the <seealso cref="RegionManager.RegionNameProperty"/>
        /// attached property from XAML.
        /// </remarks>
        protected override DependencyObject CreateShell()
        {
            Shell shell = this.Container.Resolve<Shell>();
            Application.Current.RootVisual = shell;
            return shell;
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
            return new AggregateModuleCatalog();
        }

        protected override void ConfigureModuleCatalog()
        {
            // Module A is defined in the code.
            Type moduleAType = typeof(ModuleA.ModuleA);
            ModuleCatalog.AddModule(new ModuleInfo(moduleAType.Name, moduleAType.AssemblyQualifiedName));

            // Module C is defined in the code.
            Type moduleCType = typeof(ModuleC.ModuleC);
            ModuleCatalog.AddModule(new ModuleInfo(moduleCType.Name, moduleCType.AssemblyQualifiedName));            

            // Module B, D, E and F are defined in XAML.
            IModuleCatalog xamlManifestCatalog = Microsoft.Practices.Composite.Modularity.ModuleCatalog.CreateFromXaml(new Uri("/ModularityWithUnity.Silverlight;component/ModulesCatalog.xaml", UriKind.Relative));
            ((AggregateModuleCatalog)ModuleCatalog).AddCatalog(xamlManifestCatalog);

        }        
    }
}


