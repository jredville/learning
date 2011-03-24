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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Composite.Modularity;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;

namespace Microsoft.Practices.Composite.MefExtensions.Modularity
{
    /// <summary>
    /// Component responsible for coordinating the modules' type loading and module initialization process. 
    /// </summary>
    ///  <remarks>
    /// This extends the MefModuleManager for Silverlight to provide a MEF-based XAP type loader.
    /// </remarks>
    public partial class MefModuleManager : ModuleManager, IPartImportsSatisfiedNotification
    {
        #if SILVERLIGHT
        // Due to Silverlight/MEF restrictions this must be public.
        [Import]
        public MefXapModuleTypeLoader mefXapModuleTypeLoader;
#else        
        // I import that type loader using MEF as a singleton.
        [Import]
        private MefXapModuleTypeLoader mefXapModuleTypeLoader;
#endif

        private IEnumerable<IModuleTypeLoader> mefTypeLoaders;

        public override IEnumerable<IModuleTypeLoader> ModuleTypeLoaders
        {
            get
            {
                if (this.mefTypeLoaders == null)
                {
                    this.mefTypeLoaders = new List<IModuleTypeLoader>()
                                          {
                                              mefXapModuleTypeLoader
                                          };
                }

                return this.mefTypeLoaders;
            }

            set
            {
                this.mefTypeLoaders = value;
            }
        }
    }
}
