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
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BasicMVVM.Test.Mocks;

namespace BasicMVVM.Test
{
    [TestClass]
    public class QuestionFixture
    {
        [TestMethod]
        public void WhenCreatingQuestionFromTemplateQuestionTextIsPassedThrough()
        {
            var template = new MockQuestionTemplate() { QuestionText = "ASurveyQuestion" };
            var question = new Question(template);

            Assert.AreEqual("ASurveyQuestion", question.QuestionText);
        }

        [TestMethod]
        public void WhenQuestionIsAnsweredThenIsMarkedAnswered()
        {
            var template = new MockQuestionTemplate();
            var question = new Question(template);

            question.Response = "AnAnswer";

            Assert.IsTrue(question.ResponseComplete);
        }        
    }
}
