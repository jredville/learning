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
namespace Microsoft.Practices.Composite.MefExtensions.Modularity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Linq;
    using System.Text;
    using Microsoft.Practices.Composite.Logging;
    using Microsoft.Practices.Composite.Modularity;
    using Microsoft.Practices.ServiceLocation;
    using System.ComponentModel.Composition.Hosting;
    using System.Globalization;

    /// <summary>
    /// Exports the ModuleInitializer using the Managed Extensibility Framework (MEF).
    /// </summary>
    /// <remarks>
    /// This allows the MefBootstrapper to provide this class as a default implementation.
    /// If another implementation is found, this export will not be used.
    /// </remarks>
    [Export(typeof(IModuleInitializer))]
    public class MefModuleInitializer : ModuleInitializer
    {        
#if SILVERLIGHT
        // Due to Silverlight/MEF restrictions this must be public.
        [ImportMany(AllowRecomposition = true)]
        public IEnumerable<Lazy<IModule, IModuleExport>> importedModules;
#else
        // Import the available modules from the MEF container
        [ImportMany(AllowRecomposition = true)]
        private IEnumerable<Lazy<IModule, IModuleExport>> importedModules;
#endif

        /// <summary>
        /// Initializes a new instance of the <see cref="MefModuleInitializer"/> class.
        /// </summary>
        /// <param name="serviceLocator">The container that will be used to resolve the modules by specifying its type.</param>
        /// <param name="loggerFacade">The logger to use.</param>
        [ImportingConstructor()]
        public MefModuleInitializer(IServiceLocator serviceLocator, ILoggerFacade loggerFacade)
            : base(serviceLocator, loggerFacade)
        {
        }

        protected override IModule CreateModule(string typeName)
        {
            if (this.importedModules != null && this.importedModules.Count() != 0)
            {
                Lazy<IModule, IModuleExport> lazyModule = this.importedModules.FirstOrDefault(x => (x.Metadata.ModuleType.AssemblyQualifiedName == typeName));
                if (lazyModule != null)
                {
                    return lazyModule.Value;
                }
            }

            // This does not fall back to the base implementation because the type must be in the MEF container and not just in the application domain.
            throw new ModuleInitializeException(string.Format(CultureInfo.CurrentCulture, Properties.Resources.FailedToGetType, typeName));
        }        
    }
}
