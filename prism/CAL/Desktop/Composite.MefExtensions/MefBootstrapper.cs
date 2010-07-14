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
using Microsoft.Practices.Composite.MefExtensions.Properties;

namespace Microsoft.Practices.Composite.MefExtensions
{
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;
    using System.ComponentModel.Composition.Primitives;
    using System.Reflection;
    using Microsoft.Practices.Composite.Logging;
    using Microsoft.Practices.Composite.Modularity;
    using Microsoft.Practices.ServiceLocation;

    /// <summary>
    /// Base class that provides a basic bootstrapping sequence that registers most of the Composite Application Library assets in a MEF <see cref="CompositionContainer"/>.
    /// </summary>
    /// <remarks>
    /// This class must be overriden to provide application specific configuration.
    /// </remarks>
    public abstract class MefBootstrapper : Bootstrapper
    {
        /// <summary>
        /// Gets the default <see cref="AggregateCatalog"/> for the application.
        /// </summary>
        /// <value>The default <see cref="AggregateCatalog"/> instance.</value>
        protected AggregateCatalog AggregateCatalog { get; set; }

        /// <summary>
        /// Gets the default <see cref="CompositionContainer"/> for the application.
        /// </summary>
        /// <value>The default <see cref="CompositionContainer"/> instance.</value>
        protected CompositionContainer Container { get; set; }

        /// <summary>
        /// Run the bootstrapper process.
        /// </summary>
        /// <param name="runWithDefaultConfiguration">If <see langword="true"/>, registers default 
        /// Composite Application Library services in the container. This is the default behavior.</param>
        public override void Run(bool runWithDefaultConfiguration)
        {
            this.Logger = this.CreateLogger();
            
            if (this.Logger == null)
            {
                throw new InvalidOperationException(Resources.NullLoggerFacadeException);
            }

            this.Logger.Log("Logger was created successfully.", Category.Debug, Priority.Low);

            this.Logger.Log("Creating module catalog.", Category.Debug, Priority.Low);
            this.ModuleCatalog = this.CreateModuleCatalog();
            if (this.ModuleCatalog == null)
            {
                throw new InvalidOperationException(Resources.NullModuleCatalogException);
            }
            this.Logger.Log("Configuring module catalog.", Category.Debug, Priority.Low);
            this.ConfigureModuleCatalog();

            this.Logger.Log("Creating catalog for MEF", Category.Debug, Priority.Low);
            this.AggregateCatalog = this.CreateAggregateCatalog();

            this.Logger.Log("Configuring catalog for MEF", Category.Debug, Priority.Low);
            this.ConfigureAggregateCatalog();

            this.Logger.Log("Creating Mef container", Category.Debug, Priority.Low);
            this.Container = this.CreateContainer();
            if (this.Container == null)
            {
                throw new InvalidOperationException(Resources.NullCompositionContainerException);
            }

            this.Logger.Log("Configuring MEF container", Category.Debug, Priority.Low);
            this.ConfigureContainer();
                
            this.Logger.Log("Configuring ServiceLocator singleton", Category.Debug, Priority.Low);
            this.ConfigureServiceLocator();

            this.Logger.Log("Configuring region adapters", Category.Debug, Priority.Low);
            this.ConfigureRegionAdapterMappings();

            this.Logger.Log("Configuring default region behaviors", Category.Debug, Priority.Low);
            this.ConfigureDefaultRegionBehaviors();

            this.Logger.Log("Registering Framework Exception Types", Category.Debug, Priority.Low);
            this.RegisterFrameworkExceptionTypes();

            this.Logger.Log("Creating shell", Category.Debug, Priority.Low);
            this.Shell = this.CreateShell();
            if (this.Shell != null)
            {
                this.Logger.Log("Initializing shell", Category.Debug, Priority.Low);
                this.InitializeShell();                
            }

            this.Logger.Log("Initializing modules", Category.Debug, Priority.Low);
            this.InitializeModules();

            this.Logger.Log("Bootstrapper sequence completed", Category.Debug, Priority.Low);
        }

        /// <summary>
        /// Configures the <see cref="AggregateCatalog"/> used by MEF.
        /// </summary>
        /// <remarks>
        /// The base implementation returns a new AggregateCatalog.
        /// </remarks>
        protected virtual AggregateCatalog CreateAggregateCatalog()
        {
           return new AggregateCatalog();
        }

        /// <summary>
        /// Configures the <see cref="AggregateCatalog"/> used by MEF.
        /// </summary>
        /// <remarks>
        /// The base implementation does nothing.
        /// </remarks>
        protected virtual void ConfigureAggregateCatalog()
        {
        }

        /// <summary>
        /// Creates the <see cref="CompositionContainer"/> that will be used as the default container.
        /// </summary>
        /// <returns>A new instance of <see cref="CompositionContainer"/>.</returns>
        /// <remarks>
        /// The base implementation registers a default MEF catalog of exports of key Prism types.
        /// Exporting your own types will replace these defaults.
        /// </remarks>
        protected virtual CompositionContainer CreateContainer()
        {
#if SILVERLIGHT
            ComposablePartCatalog defaultCatalog = GetComposablePartCatalog();
#else
            ComposablePartCatalog defaultCatalog = new AssemblyCatalog(Assembly.GetAssembly(typeof(MefBootstrapper)));
#endif
            
            CatalogExportProvider defaultExportProvider = new CatalogExportProvider(defaultCatalog);
            CompositionContainer container = new CompositionContainer(this.AggregateCatalog, defaultExportProvider);
            defaultExportProvider.SourceProvider = container;

            return container;
        }

#if SILVERLIGHT
        /// <summary>
        /// Returns an <see cref="AssemblyCatalog" /> for the current assembly
        /// </summary>
        /// <remarks>
        /// Due to the security restrictions in Silverlight, you cannot call
        /// <code>Assembly.GetAssembly(typeof(MefBootstrapper))</code>
        /// So, we are forced to use <code> Assembly.GetCallingAssembly()</code> instead.
        /// To ensure that the calling assembly is this one, the call is in this
        /// private helper method.
        /// </remarks>
        /// <returns>
        /// Returns an <see cref="AssemblyCatalog" /> for the current assembly
        /// </returns>
        private static ComposablePartCatalog GetComposablePartCatalog()
        {
            return new AssemblyCatalog(Assembly.GetCallingAssembly());
        }
#endif

        /// <summary>
        /// Configures the <see cref="CompositionContainer"/>. 
        /// May be overwritten in a derived class to add specific type mappings required by the application.
        /// </summary>
        /// <remarks>
        /// The base implementation registers all the types direct instantiated by the bootstrapper with the container.
        /// </remarks>
        protected virtual void ConfigureContainer()
        {
            this.Container.ComposeExportedValue<ILoggerFacade>(this.Logger);
            this.Container.ComposeExportedValue<IModuleCatalog>(this.ModuleCatalog);
            this.Container.ComposeExportedValue<CompositionContainer>(this.Container);
            this.Container.ComposeExportedValue<AggregateCatalog>(this.AggregateCatalog);
        }

        /// <summary>
        /// Configures the LocatorProvider for the <see cref="ServiceLocator" />.
        /// </summary>
        /// <remarks>
        /// The base implementation also sets the ServiceLocator provider singleton.
        /// </remarks>
        protected override void ConfigureServiceLocator()
        {
            IServiceLocator serviceLocator = this.Container.GetExportedValue<IServiceLocator>();
            ServiceLocator.SetLocatorProvider(() => serviceLocator);
        }

        /// <summary>
        /// Initializes the shell.
        /// </summary>
        /// <remarks>
        /// The base implemention ensures the shell is composed in the container.
        /// </remarks>
        protected override void InitializeShell()
        {
            this.Container.ComposeParts(this.Shell);
        }        

        /// <summary>
        /// Initializes the modules. May be overwritten in a derived class to use a custom Modules Catalog
        /// </summary>
        protected override void InitializeModules()
        {
            IModuleManager manager = this.Container.GetExportedValue<IModuleManager>();

            // TODO: comment this to explain why it is here
            this.Container.SatisfyImportsOnce(manager);
            manager.Run();
        }
    }
}
