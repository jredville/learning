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
using AcceptanceTestLibrary.Common;
using System.Windows.Automation;
using AcceptanceTestLibrary.TestEntityBase;

namespace BasicMVVM.Tests.AcceptanceTest.TestEntities.Page
{
    public static class BasicMVVMPage<TApp>
       where TApp : AppLauncherBase, new()
    {

        #region SILVERLIGHT
        public static AutomationElement Window
        {
            get { return PageBase<TApp>.Window; }
            set { PageBase<TApp>.Window = value; }
        }
        public static AutomationElement NameTextBox
        {
            get { return PageBase<TApp>.FindControlByAutomationId("NameTextBox"); }
        }

        public static AutomationElement AgeTextBox
        {
            get { return PageBase<TApp>.FindControlByAutomationId("AgeTextBox"); }
        }

        public static AutomationElement NumericQuestion
        {
            get { return PageBase<TApp>.FindControlByAutomationId("NumericQuestion"); }
        }

        public static AutomationElementCollection CheckBox
        {
            get { return PageBase<TApp>.FindAllControlsByAutomationId("CheckBox"); }
        }

        public static AutomationElementCollection OpenQuestions
        {
            get { return PageBase<TApp>.FindAllControlsByAutomationId("OpenQuestions"); }
        }
        public static AutomationElement NumberOfCharRemainingOQ1
        {
            get { return PageBase<TApp>.FindControlByContent("NumberOfCharRemainingOQ1"); }
        }

        public static AutomationElement NumberOfCharRemainingOQ2
        {
            get { return PageBase<TApp>.FindControlByContent("NumberOfCharRemainingOQ2"); }
        }

        public static AutomationElement NumberOfQuesRemaining
        {
            get { return PageBase<TApp>.FindControlByContent("NumberOfQuesRemaining"); }
        }

        public static AutomationElement AgeValidation
        {
            get { return PageBase<TApp>.FindControlByContent("ValidateAgeMessage"); }
        }

        public static AutomationElement ValidationNumericQ
        {
            get { return PageBase<TApp>.FindControlByContent("ValidateNumQ"); }
        }

        public static AutomationElement Question2Choice2
        {
            get { return PageBase<TApp>.FindControlByContent("Q2Choice2"); }
        }

        public static AutomationElement ValidationMultipleQ
        {
            get { return PageBase<TApp>.FindControlByContent("ValidationMulQ"); }
        }

        public static AutomationElement Submit
        {
            get { return PageBase<TApp>.FindControlByContent("Submit"); }
        }

        public static AutomationElement Reset
        {
            get { return PageBase<TApp>.FindControlByContent("Reset"); }
        }
        #endregion
    }
}
