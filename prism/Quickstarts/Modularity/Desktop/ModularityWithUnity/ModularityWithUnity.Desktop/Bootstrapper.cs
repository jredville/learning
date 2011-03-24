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
namespace ModularityWithUnity.Desktop
{
    using System;
    using System.Windows;
    using Microsoft.Practices.Composite.Logging;
    using Microsoft.Practices.Composite.UnityExtensions;
    using Microsoft.Practices.Unity;
    using Microsoft.Practices.Composite.Modularity;
    using Microsoft.Practices.Composite.Presentation.Regions;
    using Microsoft.Practices.Composite.Regions;
    using ModuleTracking;

    /// <summary>
    /// Initializes Prism to start this quickstart Prism application to use Unity.
    /// </summary>
    public class Bootstrapper : UnityBootstrapper
    {        
        private readonly CallbackLogger callbackLogger = new CallbackLogger();

        protected override ILoggerFacade CreateLogger()
        {
            // Because the Shell is displayed after most of the interesting boostrapper work has been performed,
            // this quickstart uses a special logger class to hold on to early log entries and display them 
            // after the UI is visible.
            return this.callbackLogger;
        }

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
        /// <see cref="UnityBootstrapper"/> will attach the default <seealso cref="IRegionManager"/> of
        /// the application in its <see cref="RegionManager.RegionManagerProperty"/> attached property
        /// in order to be able to add regions by using the <seealso cref="RegionManager.RegionNameProperty"/>
        /// attached property from XAML.
        /// </remarks>
        protected override DependencyObject CreateShell()
        {
            Shell shell = this.Container.Resolve<Shell>();
            Application.Current.MainWindow = shell;
            Application.Current.MainWindow.Show();
            return shell;
        }

        protected override void ConfigureModuleCatalog()
        {
            // Module A is defined in the code.
            Type moduleAType = typeof(ModuleA);
            ModuleCatalog.AddModule(new ModuleInfo(moduleAType.Name, moduleAType.AssemblyQualifiedName));

            // Module C is defined in the code.
            Type moduleCType = typeof(ModuleC);
            ModuleCatalog.AddModule(new ModuleInfo(moduleCType.Name, moduleCType.AssemblyQualifiedName));

            // Module B and Module D are copied to a directory as part of a post-build step.
            // These modules are not referenced in the project and are discovered by inspecting a directory.
            // Both projects have a post-build step to copy themselves into that directory.
            DirectoryModuleCatalog directoryCatalog = new DirectoryModuleCatalog() { ModulePath = @".\DirectoryModules" };
            ((AggregateModuleCatalog)ModuleCatalog).AddCatalog(directoryCatalog);

            // Module E and Module F are defined in configuration.
            ConfigurationModuleCatalog configurationCatalog = new ConfigurationModuleCatalog();
            ((AggregateModuleCatalog)ModuleCatalog).AddCatalog(configurationCatalog);

        }

        /// <summary>
        /// Returns the module catalog that will be used to initialize the modules.
        /// </summary>
        /// <returns>
        /// An instance of <see cref="IModuleCatalog"/> that will be used to initialize the modules.
        /// </returns>
        /// <remarks>
        /// When using the default initialization behavior, this method must be overwritten by a derived class.
        /// </remarks>
        protected override IModuleCatalog  CreateModuleCatalog()
        {
            AggregateModuleCatalog aggregateCatalog = new AggregateModuleCatalog();
            
            return aggregateCatalog;
        }           
    }
}
