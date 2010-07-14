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
    using System.Linq;
    using System.Text;
    using System.ComponentModel.Composition;
    using Microsoft.Practices.Composite.Modularity;
    using System.ComponentModel;

    /// <summary>
    /// An attribute that is applied to describe the Managed Extensibility Framework export of an IModule.
    /// </summary>    
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ModuleExportAttribute : ExportAttribute, IModuleExport
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ModuleExportAttribute"/> class.
        /// </summary>
        /// <param name="moduleType">The concrete type of the module being exported. Not typeof(IModule).</param>
        public ModuleExportAttribute(Type moduleType)
            : base(typeof(IModule))
        {
            this.ModuleName = moduleType.Name;
            this.ModuleType = moduleType;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ModuleExportAttribute"/> class.
        /// </summary>
        /// <param name="moduleName">The contract name of the module.</param>
        /// <param name="moduleType">The concrete type of the module being exported. Not typeof(IModule).</param>
        public ModuleExportAttribute(string moduleName, Type moduleType)
            : base(typeof(IModule))
        {
            this.ModuleName = moduleName;
            this.ModuleType = moduleType;
        }

        /// <summary>
        /// Gets the contract name of the module.
        /// </summary>
        /// <value>A string.</value>
        public string ModuleName { get; private set; }

        /// <summary>
        /// Gets concrete type of the module being exported. Not typeof(IModule).
        /// </summary>
        /// <value>A type.</value>
        public Type ModuleType { get; private set; }

        /// <summary>
        /// Gets or sets when the module should have Initialize() called.
        /// </summary>
        /// <value>An initialization mode.</value>
        public InitializationMode InitializationMode { get; set; }

        /// <summary>
        /// Gets and sets the contract names of modules this module depends upon.
        /// </summary>
        /// <value>An array of module contract names.</value>
        [DefaultValue(new string[0])]
        public string[] DependsOnModuleNames { get; set; }
    }
}
