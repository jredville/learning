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
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Reflection;
using Microsoft.Practices.Composite.Logging;
using Microsoft.Practices.Composite.MefExtensions.Modularity;
using Microsoft.Practices.Composite.Modularity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Microsoft.Practices.Composite.MefExtensions.Tests
{
    public partial class MefModuleManagerFixture
    {
        [TestMethod]
        public void ConstructorThrowsWithNullModuleInitializer()
        {
            try
            {
                new MefModuleManager(null, new Mock<IModuleCatalog>().Object, new Mock<ILoggerFacade>().Object);
                Assert.Fail("No exception thrown when expected");
            }
            catch (ArgumentNullException ex)
            {
                Assert.AreEqual("moduleInitializer", ex.ParamName);
            }
        }

        [TestMethod]
        public void ConstructorThrowsWithNullModuleCatalog()
        {
            try
            {
                new MefModuleManager(new Mock<IModuleInitializer>().Object, null, new Mock<ILoggerFacade>().Object);
                Assert.Fail("No exception thrown when expected");
            }
            catch (ArgumentNullException ex)
            {
                Assert.AreEqual("moduleCatalog", ex.ParamName);
            }
        }

        [TestMethod]
        public void ConstructorThrowsWithNullLogger()
        {
            try
            {
                new MefModuleManager(new Mock<IModuleInitializer>().Object, new Mock<IModuleCatalog>().Object, null);
                Assert.Fail("No exception thrown when expected");
            }
            catch (ArgumentNullException ex)
            {
                Assert.AreEqual("loggerFacade", ex.ParamName);
            }
        }

        [Ignore]
        [DeploymentItem(@"..\..\..\MefModulesForTesting\bin\debug\MefModulesForTesting.dll")]
        [TestMethod]
        public void ModuleInUnreferencedAssemblyInitializedByModuleInitializer()
        {
            AssemblyCatalog assemblyCatalog = new AssemblyCatalog(GetPathToModuleDll());
            CompositionContainer compositionContainer = new CompositionContainer(assemblyCatalog);

            ModuleCatalog moduleCatalog = new ModuleCatalog();
            moduleCatalog.AddModule("MefModuleOne", "MefModulesForTesting.MefModuleOne, MefModulesForTesting, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null");

            Mock<MefFileModuleTypeLoader> mockFileTypeLoader = new Mock<MefFileModuleTypeLoader>();

            compositionContainer.ComposeExportedValue<IModuleCatalog>(moduleCatalog);
            compositionContainer.ComposeExportedValue<MefFileModuleTypeLoader>(mockFileTypeLoader.Object);

            bool wasInit = false;
            var mockModuleInitializer = new Mock<IModuleInitializer>();
            mockModuleInitializer.Setup(x => x.Initialize(It.IsAny<ModuleInfo>())).Callback(() => wasInit = true);

            var mockLoggerFacade = new Mock<ILoggerFacade>();

            MefModuleManager moduleManager = new MefModuleManager(
                mockModuleInitializer.Object,
                moduleCatalog,
                mockLoggerFacade.Object);

            compositionContainer.SatisfyImportsOnce(moduleManager);

            moduleManager.Run();

            Assert.IsTrue(wasInit);
        }

        // Due to differnt test runners and file locations, this helper function will help find the 
        // necessary DLL for tests to execute.
        private static string GetPathToModuleDll()
        {
            if (File.Exists("MefModulesForTesting.dll"))
            {
                return "MefModulesForTesting.dll";
            }
            return @"..\..\..\MefModulesForTesting\bin\debug\MefModulesForTesting.dll";
        }
    }
}
