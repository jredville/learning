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
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Microsoft.Practices.Composite.Logging;
using Microsoft.Practices.Composite.Modularity;
using Microsoft.Practices.Composite.Presentation.Regions;
using Microsoft.Practices.Composite.Presentation.Regions.Behaviors;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Microsoft.Practices.Composite.MefExtensions.Tests
{
    [TestClass]
    public class MefBootstrapperFixture
    {
        [TestMethod]
        public void ContainerDefaultsToNull()
        {
            var bootstrapper = new DefaultMefBootstrapper();
            var container = bootstrapper.BaseContainer;

            Assert.IsNull(container);
        }

        [TestMethod]
        public void CanCreateConcreteBootstrapper()
        {
            new DefaultMefBootstrapper();
        }

        [TestMethod]
        public void AggregateCatalogDefaultsToNull()
        {
            var bootstrapper = new DefaultMefBootstrapper();
            Assert.IsNull(bootstrapper.BaseAggregateCatalog);
        }

        [TestMethod]
        public void CreateAggregateCatalogShouldInitializeCatalog()
        {
            var bootstrapper = new DefaultMefBootstrapper();

            bootstrapper.CallCreateAggregateCatalog();

            Assert.IsNotNull(bootstrapper.BaseAggregateCatalog);
        }

        [TestMethod]
        public void CreateContainerShouldInitializeContainer()
        {
            var bootstrapper = new DefaultMefBootstrapper();

            bootstrapper.CallCreateContainer();

            Assert.IsNotNull(bootstrapper.BaseContainer);
            Assert.IsInstanceOfType(bootstrapper.BaseContainer, typeof(CompositionContainer));
        }

        [TestMethod]
        public void CreateContainerShouldInitializeContainerProviders()
        {
            var bootstrapper = new DefaultMefBootstrapper();

            bootstrapper.CallCreateContainer();

            Assert.AreEqual(1, bootstrapper.BaseContainer.Providers.Count);
        }

        [TestMethod]
        public void ConfigureContainerAddsCompositionContainerToContainer()
        {
            var bootstrapper = new DefaultMefBootstrapper();
            bootstrapper.CallCreateLogger();
            bootstrapper.CallCreateAggregateCatalog();
            bootstrapper.CallCreateModuleCatalog();
            bootstrapper.CallCreateContainer();
            bootstrapper.CallConfigureContainer();

            var returnedContainer = bootstrapper.BaseContainer.GetExportedValue<CompositionContainer>();
            Assert.IsNotNull(returnedContainer);
            Assert.AreEqual(typeof(CompositionContainer), returnedContainer.GetType());
        }

        [TestMethod]
        public void ConfigureContainerAddsAggregateCatalogToContainer()
        {
            var bootstrapper = new DefaultMefBootstrapper();
            bootstrapper.CallCreateLogger();
            bootstrapper.CallCreateAggregateCatalog();
            bootstrapper.CallCreateModuleCatalog();
            bootstrapper.CallCreateContainer();
            bootstrapper.CallConfigureContainer();

            var returnedCatalog = bootstrapper.BaseContainer.GetExportedValue<AggregateCatalog>();
            Assert.IsNotNull(returnedCatalog);
            Assert.AreEqual(typeof(AggregateCatalog), returnedCatalog.GetType());
        }

        [TestMethod]
        public void ConfigureContainerAddsModuleCatalogToContainer()
        {
            var bootstrapper = new DefaultMefBootstrapper();
            bootstrapper.CallCreateLogger();
            bootstrapper.CallCreateAggregateCatalog();
            bootstrapper.CallCreateModuleCatalog();
            bootstrapper.CallCreateContainer();
            bootstrapper.CallConfigureContainer();

            var returnedCatalog = bootstrapper.BaseContainer.GetExportedValue<IModuleCatalog>();
            Assert.IsNotNull(returnedCatalog);
            Assert.IsTrue(returnedCatalog is IModuleCatalog);
        }

        [TestMethod]
        public void ConfigureContainerAddsLoggerFacadeToContainer()
        {
            var bootstrapper = new DefaultMefBootstrapper();
            bootstrapper.CallCreateLogger();
            bootstrapper.CallCreateAggregateCatalog();
            bootstrapper.CallCreateModuleCatalog();
            bootstrapper.CallCreateContainer();
            bootstrapper.CallConfigureContainer();

            var returnedCatalog = bootstrapper.BaseContainer.GetExportedValue<ILoggerFacade>();
            Assert.IsNotNull(returnedCatalog);
            Assert.IsTrue(returnedCatalog is ILoggerFacade);
        }

        [TestMethod]
        public void InitializeShellComposesShell()
        {
            var bootstrapper = new DefaultMefBootstrapper();
            var container = new CompositionContainer();
            var shell = new DefaultShell();

            bootstrapper.BaseContainer = container;
            bootstrapper.BaseShell = shell;

            bootstrapper.CallInitializeShell();

            Assert.IsTrue(shell.AreImportsSatisfied);
        }

        [TestMethod]
        public void ModuleManagerRunCalled()
        {
            var bootstrapper = new DefaultMefBootstrapper();
            var container = new CompositionContainer();

            Mock<IModuleManager> mockModuleManager = SetupModuleManager(container);

            bootstrapper.BaseContainer = container;

            bootstrapper.CallInitializeModules();

            mockModuleManager.Verify(mm => mm.Run(), Times.Once());
        }

        private static Mock<IModuleManager> SetupModuleManager(CompositionContainer container)
        {
            Mock<IModuleManager> mockModuleManager = new Mock<IModuleManager>();
            container.ComposeExportedValue<IModuleManager>(mockModuleManager.Object);
            return mockModuleManager;
        }

        private static void SetupRegionBehaviorAdapters(CompositionContainer container)
        {
            var regionBehaviorFactory = new RegionBehaviorFactory(new MefServiceLocatorAdapter(container));
#if SILVERLIGHT
            container.ComposeExportedValue<TabControlRegionAdapter>(new TabControlRegionAdapter(regionBehaviorFactory));
#endif

            container.ComposeExportedValue<SelectorRegionAdapter>(new SelectorRegionAdapter(regionBehaviorFactory));
            container.ComposeExportedValue<ItemsControlRegionAdapter>(new ItemsControlRegionAdapter(regionBehaviorFactory));
            container.ComposeExportedValue<ContentControlRegionAdapter>(new ContentControlRegionAdapter(regionBehaviorFactory));
            container.ComposeExportedValue<RegionAdapterMappings>(new RegionAdapterMappings());
        }

        private class DefaultShell : DependencyObject, IPartImportsSatisfiedNotification
        {
            public bool AreImportsSatisfied { get; set; }

            public void OnImportsSatisfied()
            {
                this.AreImportsSatisfied = true;
            }
        }
    }

    internal class DefaultMefBootstrapper : MefBootstrapper
    {
        public bool InitializeModulesCalled;
        public bool CreateAggregateCatalogCalled;
        public bool CreateModuleCatalogCalled;
        public bool ConfigureRegionAdapterMappingsCalled;
        public bool ConfigureAggregateCatalogCalled;
        public bool ConfigureModuleCatalogCalled;
        public bool CreateContainerCalled;
        public bool ConfigureContainerCalled;
        public bool ConfigureDefaultRegionBehaviorsCalled;
        public bool RegisterFrameworkExceptionTypesCalled;
        public bool CreateShellCalled;
        public bool InitializeShellCalled;
        public bool CreateLoggerCalled;
        public TestLogger TestLog = new TestLogger();

        public DependencyObject ShellObject = null;

        public List<string> MethodCalls = new List<string>();
        public RegionAdapterMappings DefaultRegionAdapterMappings;
        public IRegionBehaviorFactory DefaultRegionBehaviorTypes;
        public bool ConfigureServiceLocatorCalled;

        public DefaultMefBootstrapper()
        {
        }

        public ILoggerFacade BaseLogger
        {
            get
            {
                return base.Logger;
            }
        }

        public CompositionContainer BaseContainer
        {
            get { return base.Container; }
            set { base.Container = value; }
        }

        public AggregateCatalog BaseAggregateCatalog
        {
            get { return base.AggregateCatalog; }
            set { base.AggregateCatalog = value; }
        }

        public IModuleCatalog BaseModuleCatalog
        {
            get { return base.ModuleCatalog; }
            set { base.ModuleCatalog = value; }
        }

        public DependencyObject BaseShell
        {
            get { return base.Shell; }
            set { base.Shell = value; }
        }

        protected override RegionAdapterMappings ConfigureRegionAdapterMappings()
        {
            this.ConfigureRegionAdapterMappingsCalled = true;
            this.MethodCalls.Add(System.Reflection.MethodBase.GetCurrentMethod().Name);
            DefaultRegionAdapterMappings = base.ConfigureRegionAdapterMappings();

            return DefaultRegionAdapterMappings;
        }

        protected override DependencyObject CreateShell()
        {
            this.CreateShellCalled = true;
            this.MethodCalls.Add(System.Reflection.MethodBase.GetCurrentMethod().Name);
            return this.ShellObject;
        }

        protected override IRegionBehaviorFactory ConfigureDefaultRegionBehaviors()
        {
            this.ConfigureDefaultRegionBehaviorsCalled = true;
            this.MethodCalls.Add(System.Reflection.MethodBase.GetCurrentMethod().Name);
            this.DefaultRegionBehaviorTypes = base.ConfigureDefaultRegionBehaviors();

            return this.DefaultRegionBehaviorTypes;
        }

        public void CallInitializeModules()
        {
            base.InitializeModules();
        }

        public CompositionContainer CallCreateContainer()
        {
            this.Container = base.CreateContainer();
            return this.Container;
        }

        public void CallCreateAggregateCatalog()
        {
            this.AggregateCatalog = base.CreateAggregateCatalog();
        }

        public void CallCreateModuleCatalog()
        {
            this.ModuleCatalog = base.CreateModuleCatalog();
        }

        public void CallCreateLogger()
        {
            this.Logger = this.CreateLogger();
        }

        protected override ILoggerFacade CreateLogger()
        {
            this.CreateLoggerCalled = true;
            this.MethodCalls.Add(System.Reflection.MethodBase.GetCurrentMethod().Name);
            return this.TestLog;
        }

        public void CallConfigureContainer()
        {
            base.ConfigureContainer();
        }

        public void CallInitializeShell()
        {
            base.InitializeShell();
        }

        public void CallConfigureServiceLocator()
        {
            base.ConfigureServiceLocator();
        }

        protected override AggregateCatalog CreateAggregateCatalog()
        {
            this.CreateAggregateCatalogCalled = true;
            this.MethodCalls.Add(System.Reflection.MethodBase.GetCurrentMethod().Name);
            return base.CreateAggregateCatalog();
        }

        protected override void ConfigureAggregateCatalog()
        {
            this.ConfigureAggregateCatalogCalled = true;
            this.MethodCalls.Add(System.Reflection.MethodBase.GetCurrentMethod().Name);
            // no op
        }

        protected override void ConfigureModuleCatalog()
        {
            this.ConfigureModuleCatalogCalled = true;
            this.MethodCalls.Add(System.Reflection.MethodBase.GetCurrentMethod().Name);
            base.ConfigureModuleCatalog();
        }

        protected override CompositionContainer CreateContainer()
        {
            this.CreateContainerCalled = true;
            this.MethodCalls.Add(System.Reflection.MethodBase.GetCurrentMethod().Name);
            return base.CreateContainer();
        }

        protected override void ConfigureContainer()
        {
            this.ConfigureContainerCalled = true;
            this.MethodCalls.Add(System.Reflection.MethodBase.GetCurrentMethod().Name);
            base.ConfigureContainer();
        }

        protected override void RegisterFrameworkExceptionTypes()
        {
            this.RegisterFrameworkExceptionTypesCalled = true;
            this.MethodCalls.Add(System.Reflection.MethodBase.GetCurrentMethod().Name);
        }

        protected override void InitializeShell()
        {
            this.MethodCalls.Add(System.Reflection.MethodBase.GetCurrentMethod().Name);
            this.InitializeShellCalled = true;
            base.InitializeShell();
        }

        protected override void InitializeModules()
        {
            this.MethodCalls.Add(System.Reflection.MethodBase.GetCurrentMethod().Name);
            this.InitializeModulesCalled = true;
            base.InitializeModules();
        }

        protected override IModuleCatalog CreateModuleCatalog()
        {
            this.CreateModuleCatalogCalled = true;
            this.MethodCalls.Add(System.Reflection.MethodBase.GetCurrentMethod().Name);
            return base.CreateModuleCatalog();
        }

        protected override void ConfigureServiceLocator()
        {
            this.ConfigureServiceLocatorCalled = true;
            this.MethodCalls.Add(System.Reflection.MethodBase.GetCurrentMethod().Name);
            base.ConfigureServiceLocator();
        }
    }
}
