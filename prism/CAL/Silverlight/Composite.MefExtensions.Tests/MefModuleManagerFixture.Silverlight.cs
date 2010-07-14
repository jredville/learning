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
using System.ComponentModel.Composition.Hosting;
using System.Text.RegularExpressions;
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
                Assert.AreEqual("moduleInitializer", ex.GetParameterName());
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
                Assert.AreEqual("moduleCatalog", ex.GetParameterName());
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
                Assert.AreEqual("loggerFacade", ex.GetParameterName());
            }
        }
    }

    public static class ArgumentNullExceptionExtensions
    {
        public static string GetParameterName(this ArgumentNullException exception)
        {
            const string markerText = "Parameter name: ";
            string message = exception.Message;
            int index = message.LastIndexOf(markerText);

            string parameterName = message.Substring(index+markerText.Length);

            return parameterName;
        }
    }
}