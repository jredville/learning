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
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Reflection;
using Microsoft.Practices.Composite.Logging;
using Microsoft.Practices.Composite.MefExtensions.Modularity;
using Microsoft.Practices.Composite.Modularity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Microsoft.Practices.Composite.MefExtensions.Tests
{
    

    [TestClass]
    public partial class MefModuleManagerFixture
    {
        [TestMethod]
        public void ModuleNeedsRetrievalReturnsTrueWhenModulesAreNotImported()
        {
            TestableMefModuleManager moduleManager = new TestableMefModuleManager();
            bool result = moduleManager.CallModuleNeedsRetrieval(new ModuleInfo("name", "type"));

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ModuleNeedsRetrievalReturnsTrueWhenNoModulesAreAvailable()
        {
            TestableMefModuleManager moduleManager = new TestableMefModuleManager
                                                         {
                                                             Modules = (IEnumerable<Lazy<IModule, IModuleExport>>) new List<Lazy<IModule,  IModuleExport>>()
                                                         };

            bool result = moduleManager.CallModuleNeedsRetrieval(new ModuleInfo("name", "type"));

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ModuleNeedsRetrievalReturnsTrueWhenModuleNotFound()
        {
            TestableMefModuleManager moduleManager = new TestableMefModuleManager
            {
                Modules = new List<Lazy<IModule, IModuleExport>>() { new Lazy<IModule, IModuleExport>(() => new TestModule(), new TestModuleMetadata()) }
            };

            bool result = moduleManager.CallModuleNeedsRetrieval(new ModuleInfo("name", "type"));

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ModuleNeedsRetrievalReturnsFalseWhenModuleIsFound()
        {
            TestableMefModuleManager moduleManager = new TestableMefModuleManager
            {
                Modules = new List<Lazy<IModule, IModuleExport>>() { new Lazy<IModule, IModuleExport>(() => new TestModule(), new TestModuleMetadata()) }
            };

            bool result = moduleManager.CallModuleNeedsRetrieval(new ModuleInfo("TestModule", "Microsoft.Practices.Composite.MefExtensions.Tests.MefModuleManagerFixture.TestModule"));

            Assert.IsFalse(result);
        }
                
        public class TestModule : IModule
        {
            public void Initialize()
            {
                //no-op
            }
        }

        public class TestModuleMetadata : IModuleExport
        {
            public string ModuleName
            {
                get { return "TestModule"; }
            }

            public Type ModuleType
            {
                get { return typeof(TestModule); }
            }

            public InitializationMode InitializationMode
            {
                get { return InitializationMode.WhenAvailable; }
            }

            public string[] DependsOnModuleNames
            {
                get { return null; }
            }
        }

        internal class TestableMefModuleManager : MefModuleManager
        {
            public TestableMefModuleManager()
                : base(new Mock<IModuleInitializer>().Object, new Mock<IModuleCatalog>().Object, new Mock<ILoggerFacade>().Object)
            {
            }

            public IEnumerable<Lazy<IModule, IModuleExport>> Modules
            {
                get { return base.ImportedModules; }
                set { base.ImportedModules = value; }
            }

            public bool CallModuleNeedsRetrieval(ModuleInfo moduleInfo)
            {
                return base.ModuleNeedsRetrieval(moduleInfo);
            }
        }
    }
}