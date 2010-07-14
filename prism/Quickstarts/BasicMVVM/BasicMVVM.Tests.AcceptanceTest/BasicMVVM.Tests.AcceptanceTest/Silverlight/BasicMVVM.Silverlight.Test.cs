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
using BasicMVVM.Tests.AcceptanceTest.TestEntities.Page;
using System.Threading;
using AcceptanceTestLibrary.TestEntityBase;
using AcceptanceTestLibrary.ApplicationHelper;
using System.IO;
using System;
using System.Windows.Automation;
using AcceptanceTestLibrary.UIAWrapper;

namespace BasicMVVM.Tests.AcceptanceTest.Silverlight
{

#if DEBUG

    [DeploymentItem(@".\BasicMVVM.Tests.AcceptanceTest\bin\Debug")]
    [DeploymentItem(@"..\BasicMVVM\Bin\Debug", "SL")]
#else
    [DeploymentItem(@".\BasicMVVM.Tests.AcceptanceTest\bin\Release")]
    [DeploymentItem(@"..\BasicMVVM\Bin\Release", "SL")]
#endif

    [TestClass]
    public class BasicMVVM_Silverlight_Tests : FixtureBase<SilverlightAppLauncher>
    {

        private const int BACKTRACKLENGTH = 4;

        #region Additional test attributes
        // Use TestInitialize to run code before running each test 
        [TestInitialize()]
        public void MyTestInitialize()
        {
            string currentOutputPath = (new System.IO.DirectoryInfo(Assembly.GetExecutingAssembly().Location)).Parent.FullName;
            BasicMVVMPage<SilverlightAppLauncher>.Window = base.LaunchApplication(currentOutputPath + GetSilverlightApplication(), GetBrowserTitle())[0];
            Thread.Sleep(5000);
        }

        // Use TestCleanup to run code after each test has run
        [TestCleanup()]
        public void MyTestCleanup()
        {
            PageBase<SilverlightAppLauncher>.DisposeWindow();
            SilverlightAppLauncher.UnloadBrowser(GetBrowserTitle());
        }

        #endregion

        #region Test Methods

        /// <summary>
        /// Tests if the Silverlight MVVM QS is launched.
        /// </summary>
        [TestMethod]
        public void SilverlightBasicMVVMLaunchTest()
        {
            Assert.IsNotNull(BasicMVVMPage<SilverlightAppLauncher>.Window, "Basic MVVM QS application is not launched.");
        }
        [TestMethod]
        public void SilverlightEnterTextInNameField()
        {
            AutomationElement aeNameTextBox = BasicMVVMPage<SilverlightAppLauncher>.NameTextBox;
            Assert.IsNotNull(aeNameTextBox, "Name field is not loaded");
            aeNameTextBox.SetValue(new ResXConfigHandler(ConfigHandler.GetValue("TestDataInputFile")).GetValue("Name"));
            Thread.Sleep(2000);
            Assert.AreEqual(aeNameTextBox.GetValue().ToString(), new ResXConfigHandler(ConfigHandler.GetValue("TestDataInputFile")).GetValue("Name"));
        }
        [TestMethod]
        public void SilverlightValidateAge()
        {
            //Positive test case to validate Age
            AutomationElement aeAgeTextBox = BasicMVVMPage<SilverlightAppLauncher>.AgeTextBox;
            Assert.IsNotNull(aeAgeTextBox, "Age Field is not loaded");
            aeAgeTextBox.SetFocus();
            aeAgeTextBox.SetValue(new ResXConfigHandler(ConfigHandler.GetValue("TestDataInputFile")).GetValue("ValidAge"));
            Thread.Sleep(2000);
            Assert.AreEqual(aeAgeTextBox.GetValue().ToString(), new ResXConfigHandler(ConfigHandler.GetValue("TestDataInputFile")).GetValue("ValidAge"));
            AutomationElement aeValidateAgeP = BasicMVVMPage<SilverlightAppLauncher>.AgeValidation;
            Assert.IsNull(aeValidateAgeP, "Validation Error for Age");
            //AutomationElement aeValidation = BasicMVVMPage<SilverlightAppLauncher>.ValidationSummary;
            //Assert.IsTrue(aeValidation.Current.Name == new ResXConfigHandler(ConfigHandler.GetValue("TestDataInputFile")).GetValue("DefaultValidationText"), "Age  - Validation error");

            //Negative Test Case
            aeAgeTextBox.SetFocus();
            aeAgeTextBox.SetValue(new ResXConfigHandler(ConfigHandler.GetValue("TestDataInputFile")).GetValue("InvalidAge"));
            Thread.Sleep(2000);

            AutomationElement aeNameTextBox = BasicMVVMPage<SilverlightAppLauncher>.NameTextBox;
            aeNameTextBox.SetValue(" ");
            aeAgeTextBox.SetFocus();
            AutomationElement aeValidateAge = BasicMVVMPage<SilverlightAppLauncher>.AgeValidation;
            Assert.IsNotNull(aeValidateAge, "No Validation for Age");
        }

        [TestMethod]
        public void SilverlightAnswerOpenQuestion1()
        {
            String validText = new ResXConfigHandler(ConfigHandler.GetValue("TestDataInputFile")).GetValue("ValidText");
            AutomationElementCollection aeOpenQuestions = BasicMVVMPage<SilverlightAppLauncher>.OpenQuestions;
            Assert.IsNotNull(aeOpenQuestions[0], "Open Question 1 is loaded");
            aeOpenQuestions[0].SetFocus();
            aeOpenQuestions[0].SetValue(validText);
            Thread.Sleep(2000);
            Assert.AreEqual(aeOpenQuestions[0].GetValue().ToString(), validText);

            //Check Number of characters remaining
            AutomationElement aeNumberOfCharRemaining = BasicMVVMPage<SilverlightAppLauncher>.NumberOfCharRemainingOQ1;
            Assert.IsNotNull(aeNumberOfCharRemaining, "Number Of Char Remaining for Question1 is not loaded Or calculation might be wrong");
        }

        [TestMethod]
        public void SilverlightAnswerNumericQuestion()
        {
            AutomationElement aeNumQuestion = BasicMVVMPage<SilverlightAppLauncher>.NumericQuestion;
            Assert.IsNotNull(aeNumQuestion, "Numeric Question is not loaded");
            aeNumQuestion.SetFocus();
            aeNumQuestion.SetValue(new ResXConfigHandler(ConfigHandler.GetValue("TestDataInputFile")).GetValue("ValidNumber"));
            Thread.Sleep(2000);
            AutomationElement aeValidatePositiveNumQ = BasicMVVMPage<SilverlightAppLauncher>.ValidationNumericQ;
            Assert.IsNull(aeValidatePositiveNumQ, "Validation Error for Numeric Question");            

            //Negative Test Case
            aeNumQuestion.SetFocus();
            aeNumQuestion.SetValue(new ResXConfigHandler(ConfigHandler.GetValue("TestDataInputFile")).GetValue("InvalidNumber"));
            Thread.Sleep(2000);
            AutomationElement aeValidateNumQ = BasicMVVMPage<SilverlightAppLauncher>.ValidationNumericQ;
            Assert.IsNotNull(aeValidateNumQ, "No Validation for Numeric Question");
        }

        [TestMethod]
        public void SilverlightValidateMultipleAnsQ()
        {
            AutomationElementCollection aeQuestion2 = BasicMVVMPage<SilverlightAppLauncher>.CheckBox;
            Assert.IsNotNull(aeQuestion2[1], "Choice 2 is not loaded");
            aeQuestion2[1].SetFocus();
            System.Windows.Forms.SendKeys.SendWait(" ");
            Thread.Sleep(1000);
            aeQuestion2[2].SetFocus();
            System.Windows.Forms.SendKeys.SendWait(" ");
            Thread.Sleep(1000);
            aeQuestion2[3].SetFocus();
            System.Windows.Forms.SendKeys.SendWait(" ");
            Thread.Sleep(1000);

            AutomationElement aeValidateQuestion2 = BasicMVVMPage<SilverlightAppLauncher>.ValidationMultipleQ;
            Assert.IsNotNull(aeValidateQuestion2, "No Validation for Multiple Answer Question");
        }

        [TestMethod]
        public void SilverlightValidateOpenQuestion2()
        {
            String textwithSpecChar = new ResXConfigHandler(ConfigHandler.GetValue("TestDataInputFile")).GetValue("TextWithSpecChars");
            AutomationElementCollection aeOpenQuestions2 = BasicMVVMPage<SilverlightAppLauncher>.OpenQuestions;
            Assert.IsNotNull(aeOpenQuestions2[1], "Open Question 2 is not loaded");
            aeOpenQuestions2[1].SetFocus();
            aeOpenQuestions2[1].SetValue(new ResXConfigHandler(ConfigHandler.GetValue("TestDataInputFile")).GetValue("TextWithSpecChars"));
            Thread.Sleep(2000);
            Assert.AreEqual(aeOpenQuestions2[1].GetValue().ToString(), textwithSpecChar);

            //Check Number of characters remaining
            AutomationElement aeNumberOfCharRemaining = BasicMVVMPage<SilverlightAppLauncher>.NumberOfCharRemainingOQ2;
            Assert.IsNotNull(aeNumberOfCharRemaining, "Number Of Char Remaining for Open Question 2 is not loaded Or calculation might be wrong");

            //Check that maximum of 25 characters can be entered
            //aeOpenQuestions2[1].SetValue(new ResXConfigHandler(ConfigHandler.GetValue("TestDataInputFile")).GetValue("MaxCharsQ3"));
            //Thread.Sleep(10000);
            //Assert.AreEqual(aeOpenQuestions2[1].GetValue().ToString(), new ResXConfigHandler(ConfigHandler.GetValue("TestDataInputFile")).GetValue("MaxCharsAllowedQ3"));

        }


        [TestMethod]
        public void SilverlightValidateSubmitAndReset()
        {
            //Submit button validation
            FillAllDetails();
            AutomationElement aeSubmit = BasicMVVMPage<SilverlightAppLauncher>.Submit;
            Assert.IsTrue(aeSubmit.Current.IsEnabled, "Submit Button is not enabled");
            aeSubmit.Click();
            CheckAllFieldsCleared();

            //Reset button validation
            FillAllDetails();
            AutomationElement aeReset = BasicMVVMPage<SilverlightAppLauncher>.Reset;
            Assert.IsTrue(aeReset.Current.IsEnabled, "Reset Button is not enabled");
            aeReset.Click();
            CheckAllFieldsCleared();


        }

        [TestMethod]
        public void SilverlightvalidateItemsRemaining()
        {
            String validText = new ResXConfigHandler(ConfigHandler.GetValue("TestDataInputFile")).GetValue("ValidText");
            AutomationElementCollection aeOpenQuestions = BasicMVVMPage<SilverlightAppLauncher>.OpenQuestions;
            aeOpenQuestions[0].SetValue(validText);
            Thread.Sleep(1000);

            AutomationElement aeNumberOfCharRemaining = BasicMVVMPage<SilverlightAppLauncher>.NumberOfQuesRemaining;
            Assert.IsNotNull(aeNumberOfCharRemaining, "Number Of Questions Remaining is not loaded Or calculation might be wrong");

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

        private void FillAllDetails()
        {
            AutomationElement aeNameTextBox = BasicMVVMPage<SilverlightAppLauncher>.NameTextBox;
            aeNameTextBox.SetValue(new ResXConfigHandler(ConfigHandler.GetValue("TestDataInputFile")).GetValue("Name"));
            Thread.Sleep(1000);

            AutomationElement aeAgeTextBox = BasicMVVMPage<SilverlightAppLauncher>.AgeTextBox;
            aeAgeTextBox.SetValue(new ResXConfigHandler(ConfigHandler.GetValue("TestDataInputFile")).GetValue("ValidAge"));
            Thread.Sleep(1000);


            String validText = new ResXConfigHandler(ConfigHandler.GetValue("TestDataInputFile")).GetValue("ValidText");
            AutomationElementCollection aeQuestion1 = BasicMVVMPage<SilverlightAppLauncher>.OpenQuestions;
            aeQuestion1[0].SetValue(validText);
            Thread.Sleep(1000);

            AutomationElement aeQuestion4 = BasicMVVMPage<SilverlightAppLauncher>.NumericQuestion;
            aeQuestion4.SetValue(new ResXConfigHandler(ConfigHandler.GetValue("TestDataInputFile")).GetValue("ValidNumber"));
            Thread.Sleep(1000);

            AutomationElementCollection aeQuestion2 = BasicMVVMPage<SilverlightAppLauncher>.CheckBox;
            Assert.IsNotNull(aeQuestion2[1], "Choice 2 is not loaded");
            aeQuestion2[1].SetFocus();
            System.Windows.Forms.SendKeys.SendWait(" ");
            Thread.Sleep(1000);

            String textwithSpecChar = new ResXConfigHandler(ConfigHandler.GetValue("TestDataInputFile")).GetValue("TextWithSpecChars");
            AutomationElementCollection aeOpenQuestions2 = BasicMVVMPage<SilverlightAppLauncher>.OpenQuestions;
            aeOpenQuestions2[1].SetValue(new ResXConfigHandler(ConfigHandler.GetValue("TestDataInputFile")).GetValue("TextWithSpecChars"));
            Thread.Sleep(1000);
        }

        private void CheckAllFieldsCleared()
        {
            AutomationElement aeNameTextBox = BasicMVVMPage<SilverlightAppLauncher>.NameTextBox;
            Thread.Sleep(1000);
            Assert.IsTrue(aeNameTextBox.GetValue().Equals(""), "Name Field is not cleared");

            AutomationElement aeAgeTextBox = BasicMVVMPage<SilverlightAppLauncher>.AgeTextBox;
            Thread.Sleep(1000);
            Assert.IsTrue(aeAgeTextBox.GetValue().Equals(""), "Age Field is not cleared");

            AutomationElementCollection aeOpenQuestions = BasicMVVMPage<SilverlightAppLauncher>.OpenQuestions;
            Thread.Sleep(1000);
            Assert.IsTrue(aeOpenQuestions[0].GetValue().Equals(""), "Open Question 1 Field is not cleared");
            Thread.Sleep(1000);
            Assert.IsTrue(aeOpenQuestions[1].GetValue().Equals(""), "Open Question 2 Field is not cleared");

            AutomationElement aeQuestion4 = BasicMVVMPage<SilverlightAppLauncher>.NumericQuestion;
            Thread.Sleep(1000);
            Assert.IsTrue(aeQuestion4.GetValue().Equals(""), "Question 4 Field is not cleared");

            //AutomationElementCollection aeQuestion2 = BasicMVVMPage<SilverlightAppLauncher>.CheckBox;
            //foreach (AutomationElement ae in aeQuestion2)
            //{
            //    Assert.IsTrue(ae.GetValue().Equals(""), "Question 2 Checkboxes are not cleared");
            //}            

         
           
        }
        #endregion
    }
}
