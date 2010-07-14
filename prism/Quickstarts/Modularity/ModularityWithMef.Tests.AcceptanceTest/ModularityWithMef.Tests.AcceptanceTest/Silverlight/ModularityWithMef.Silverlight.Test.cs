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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AcceptanceTestLibrary.Common;
using AcceptanceTestLibrary.Common.Silverlight;
using System.Reflection;
using ModularityWithMef.Tests.AcceptanceTest.TestEntities.Page;
using System.Threading;
using AcceptanceTestLibrary.TestEntityBase;
using AcceptanceTestLibrary.ApplicationHelper;
using System.IO;
using System;
using System.Windows.Automation;
using AcceptanceTestLibrary.UIAWrapper;
using System.Text.RegularExpressions;

namespace ModularityWithMef.Tests.AcceptanceTest
{

#if DEBUG
    [DeploymentItem(@".\ModularityWithMef.Tests.AcceptanceTest\bin\Debug")]
    [DeploymentItem(@"..\Silverlight\ModularityWithMef\ModularityWithMef.Silverlight\bin\Debug", "SL")]
#else
    [DeploymentItem(@".\ModularityWithMef.Tests.AcceptanceTest\bin\Release")]
    [DeploymentItem(@"..\Desktop\ModularityWithMef\ModularityWithMef.Desktop\bin\Release", "WPF")]
#endif

    [TestClass]
    public class MefSilverlightTests : FixtureBase<SilverlightAppLauncher>
    {

        private const int BACKTRACKLENGTH = 5;

        #region Additional test attributes
        // Use TestInitialize to run code before running each test 
        [TestInitialize()]
        public void MyTestInitialize()
        {
            //string currentOutputPath = (new System.IO.DirectoryInfo(Assembly.GetExecutingAssembly().Location)).Parent.FullName;
            //ModularityPage<SilverlightAppLauncher>.Window = base.LaunchApplication(currentOutputPath + GetSilverlightApplication(), GetBrowserTitle())[0];
            //Thread.Sleep(5000);

            string pp = GetAbsolutePath(BACKTRACKLENGTH);

            //Parameter list as follows.
            //1. Port number of host application 2. Absolute path of the Silverlight Host.

            base.StartWebServer(GetPortNumber(GetSilverlightApplication()), GetAbsolutePath(BACKTRACKLENGTH) + GetSilverlightApplicationHostPath());
            ModularityPage<SilverlightAppLauncher>.Window = base.LaunchApplication(GetSilverlightApplication(), GetBrowserTitle())[0];
            Thread.Sleep(5000);
        }

        // Use TestCleanup to run code after each test has run
        [TestCleanup()]
        public void MyTestCleanup()
        {
            PageBase<SilverlightAppLauncher>.DisposeWindow();
            base.StopWebServer();
            SilverlightAppLauncher.UnloadBrowser(GetBrowserTitle());
           
        }

        #endregion

        #region Test Methods

        /// <summary>
        /// Tests if the Silverlight MVVM QS is launched.
        /// </summary>
        [TestMethod]
        public void SilverlightModularityLaunchTest()
        {
            Assert.IsNotNull(ModularityPage<SilverlightAppLauncher>.Window, "Modularity SL application is not launched.");
        }

        [TestMethod]
        public void SilverlightModulesLoadedOnStartup()
        {
            AutomationElement aeModuleA = ModularityPage<SilverlightAppLauncher>.ModuleAContent;
            Thread.Sleep(1000);
            Assert.IsNotNull(aeModuleA);

           

            AutomationElement aeModuleD = ModularityPage<SilverlightAppLauncher>.ModuleDContent;
            Thread.Sleep(1000);
            Assert.IsNotNull(aeModuleD);

            //Get the clickable point on D and click on it.           
            System.Windows.Point pointD =
                new System.Windows.Point(aeModuleD.Current.BoundingRectangle.X + aeModuleD.Current.BoundingRectangle.Width / 2,
                    aeModuleD.Current.BoundingRectangle.Y + aeModuleD.Current.BoundingRectangle.Height / 2);
            System.Windows.Forms.Cursor.Position = new System.Drawing.Point((int)pointD.X, (int)pointD.Y);
            Thread.Sleep(2000);
            AutomationElement aeModDtooltip = ModularityPage<SilverlightAppLauncher>.DownloadedModuletooltip;
            Assert.IsFalse(aeModDtooltip.Current.IsOffscreen, "Module D Tooltip is offscreen");


            //Get the clickable point on A and click on it.           
            System.Windows.Point point =
                new System.Windows.Point(aeModuleA.Current.BoundingRectangle.X + aeModuleA.Current.BoundingRectangle.Width / 2,
                    aeModuleA.Current.BoundingRectangle.Y + aeModuleA.Current.BoundingRectangle.Height / 2);
            System.Windows.Forms.Cursor.Position = new System.Drawing.Point((int)point.X, (int)point.Y);
            Thread.Sleep(2000);
            AutomationElement aeModAtooltip = ModularityPage<SilverlightAppLauncher>.IntializedModuletooltip;
            Assert.IsFalse(aeModAtooltip.Current.IsOffscreen, "Module A Tooltip is offscreen");

        }

        [TestMethod]
        public void SilverlightModuleCLoadedOnDemand()
        {
            AutomationElement aeModuleC = ModularityPage<SilverlightAppLauncher>.ModuleCContent;
            Thread.Sleep(1000);
            Assert.IsNotNull(aeModuleC);
           
            //Get the clickable point and click on it.           
            System.Windows.Point point =
                new System.Windows.Point(aeModuleC.Current.BoundingRectangle.X + aeModuleC.Current.BoundingRectangle.Width / 2,
                    aeModuleC.Current.BoundingRectangle.Y + aeModuleC.Current.BoundingRectangle.Height / 2);
            System.Windows.Forms.Cursor.Position = new System.Drawing.Point((int)point.X, (int)point.Y);
            Thread.Sleep(2000);
            MouseEvents.Click();
            MouseEvents.Click();
            Thread.Sleep(1000);

            System.Windows.Point point1 =
              new System.Windows.Point(aeModuleC.Current.BoundingRectangle.Width / 2,
                   aeModuleC.Current.BoundingRectangle.Height / 2);
            System.Windows.Forms.Cursor.Position = new System.Drawing.Point((int)point1.X, (int)point1.Y);
            Thread.Sleep(1000);
            System.Windows.Forms.Cursor.Position = new System.Drawing.Point((int)point.X, (int)point.Y);
            Thread.Sleep(3000);
            AutomationElement aeModCtooltip = ModularityPage<SilverlightAppLauncher>.IntializedModuletooltip;
            Assert.IsFalse(aeModCtooltip.Current.IsOffscreen, "Module C Tooltip is offscreen");

         }

        [TestMethod]
        public void SilverlightLoadModuleFAndDependency()
        {
            AutomationElement aeModuleF = ModularityPage<SilverlightAppLauncher>.ModuleFContent;
            Thread.Sleep(1000);
            Assert.IsNotNull(aeModuleF);

            //Get the clickable point and click on it.           
            System.Windows.Point pointF =
                new System.Windows.Point(aeModuleF.Current.BoundingRectangle.X + aeModuleF.Current.BoundingRectangle.Width / 2,
                    aeModuleF.Current.BoundingRectangle.Y + aeModuleF.Current.BoundingRectangle.Height / 2);
            System.Windows.Forms.Cursor.Position = new System.Drawing.Point((int)pointF.X, (int)pointF.Y);
            Thread.Sleep(2000);
            MouseEvents.Click();
            MouseEvents.Click();
            Thread.Sleep(10000);

            //Get control of Module  E 
            AutomationElement aeModuleE = ModularityPage<SilverlightAppLauncher>.ModuleEContent;
            Thread.Sleep(3000);
            Assert.IsNotNull(aeModuleE);
            System.Windows.Point pointE =
               new System.Windows.Point(aeModuleE.Current.BoundingRectangle.X + aeModuleE.Current.BoundingRectangle.Width / 2,
                   aeModuleE.Current.BoundingRectangle.Y + aeModuleE.Current.BoundingRectangle.Height / 2);
            Thread.Sleep(1000);
            System.Windows.Forms.Cursor.Position = new System.Drawing.Point((int)pointE.X, (int)pointE.Y);
            Thread.Sleep(2000);
            AutomationElement aeModEtooltip = ModularityPage<SilverlightAppLauncher>.DownloadedModuletooltip;
            Assert.IsFalse(aeModEtooltip.Current.IsOffscreen, "Module E Tooltip is offscreen");


            //System.Windows.Point pointH =
            //  new System.Windows.Point(aeModuleF.Current.BoundingRectangle.Width / 2,
            //       aeModuleF.Current.BoundingRectangle.Height / 2);
            //System.Windows.Forms.Cursor.Position = new System.Drawing.Point((int)pointH.X, (int)pointH.Y);
            Thread.Sleep(1000);
            System.Windows.Forms.Cursor.Position = new System.Drawing.Point((int)pointF.X, (int)pointF.Y);
            Thread.Sleep(3000);
            AutomationElement aeModFtooltip = ModularityPage<SilverlightAppLauncher>.DownloadedModuletooltip;
            Assert.IsFalse(aeModFtooltip.Current.IsOffscreen, "Module F Tooltip is offscreen");

          
        }


        #endregion

        #region Helper Methods
        private static string GetSilverlightApplication()
        {
            return ConfigHandler.GetValue("SilverlightAppLocation");
        }

        private static string GetSilverlightApplicationPath(int backTrackLength)
        {
            string currentDirectory = Directory.GetCurrentDirectory();
            if (!String.IsNullOrEmpty(currentDirectory) && Directory.Exists(currentDirectory))
            {
                for (int iIndex = 0; iIndex < backTrackLength; iIndex++)
                {
                    currentDirectory = Directory.GetParent(currentDirectory).ToString();
                }
            }
            return currentDirectory + GetSilverlightApplication();
        }

        private static string GetBrowserTitle()
        {
            return new ResXConfigHandler(ConfigHandler.GetValue("ControlIdentifiersFile")).GetValue("SilverlightAppTitle");
        }

        private static string GetAbsolutePath(int backTrackLength)
        {
            string currentDirectory = Directory.GetCurrentDirectory();
            if (!String.IsNullOrEmpty(currentDirectory) && Directory.Exists(currentDirectory))
            {
                for (int iIndex = 0; iIndex < backTrackLength; iIndex++)
                {
                    currentDirectory = Directory.GetParent(currentDirectory).ToString();
                }
            }

            return currentDirectory;
        }
        

        /// <summary>
        /// Extract the Port number from a URL of the format http://ServerName:PortNumber/SitePath
        /// </summary>
        /// <param name="url">URL of the above format</param>
        /// <returns>port number sub-string</returns>
        private static string GetPortNumber(string url)
        {
            string urlPattern = @"^(?<proto>\w+)://[^/]+?(?<port>:\d+)?/";
            Regex urlExpression = new Regex(urlPattern, RegexOptions.Compiled);
            Match urlMatch = urlExpression.Match(url);
            return urlMatch.Groups["port"].Value;
        }

        private static string GetSilverlightApplicationHostPath()
        {
            return ConfigHandler.GetValue("SilverlightAppHostRelativeLocation");
        }
        #endregion
    }
}
