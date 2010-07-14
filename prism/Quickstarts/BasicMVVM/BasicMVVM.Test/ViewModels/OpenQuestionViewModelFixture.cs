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
using System.Linq;
using BasicMVVM.Test.Mocks;
using BasicMVVM.Test.ViewModels.Utility;
using BasicMVVM.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BasicMVVM.Test.ViewModels
{
    [TestClass]
    public class OpenQuestionViewModelFixture
    {
        [TestMethod]
        public void WhenCreatingAViewModelWithAQuestionForADifferentTemplateAnExceptionIsThrown()
        {
            var question = new Question(new MockQuestionTemplate());

            try
            {
                new OpenQuestionViewModel(question);
                Assert.Fail("should have thrown");
            }
            catch (ArgumentException)
            {
                // expected
            }
        }

        [TestMethod]
        public void ViewModelGetsQuestionTextAndMaxLengthFromTheTemplate()
        {
            var question = new Question(new OpenQuestionTemplate { QuestionText = "Question", MaxLength = 25 });
            var viewModel = new OpenQuestionViewModel(question);

            Assert.AreEqual("Question", viewModel.QuestionText);
            Assert.AreEqual(25, viewModel.MaxLength);
        }

        [TestMethod]
        public void ViewModelHasMaxLengthAsTheAvailableLength()
        {
            var question = new Question(new OpenQuestionTemplate { QuestionText = "Question", MaxLength = 25 });
            var viewModel = new OpenQuestionViewModel(question);

            Assert.AreEqual(25, viewModel.AvailableLength);
        }

        [TestMethod]
        public void WhenSettingTheTextPropertyOnTheViewModelTheResponsePropertyIsSetOnTheQuestion()
        {
            var question = new Question(new OpenQuestionTemplate { QuestionText = "Question", MaxLength = 25 });
            var viewModel = new OpenQuestionViewModel(question);

            viewModel.ResponseText = "response";

            Assert.AreEqual("response", question.Response);
            Assert.AreEqual("response", viewModel.ResponseText);
        }

        [TestMethod]
        public void WhenSettingTheTextPropertyOnTheViewModelThenAvailableLengthIsUpdated()
        {
            var question = new Question(new OpenQuestionTemplate { QuestionText = "Question", MaxLength = 25 });
            var viewModel = new OpenQuestionViewModel(question);

            viewModel.ResponseText = "1234567890";

            Assert.AreEqual(15, viewModel.AvailableLength);
        }

        [TestMethod]
        public void WhenSettingTheTextPropertyOnTheViewModelThenAChangeOnAvailableLengthIsNotified()
        {
            var question = new Question(new OpenQuestionTemplate { QuestionText = "Question", MaxLength = 25 });
            var viewModel = new OpenQuestionViewModel(question);

            var changeTracker = new PropertyChangeTracker(viewModel);
            viewModel.ResponseText = "1234567890";

            Assert.IsTrue(changeTracker.ChangedProperties.Contains("AvailableLength"));
        }

        [TestMethod]
        public void WhenSettingTheTextPropertyOnTheViewModelThenAChangeOnResponseTextIsNotified()
        {
            var question = new Question(new OpenQuestionTemplate { QuestionText = "Question", MaxLength = 25 });
            var viewModel = new OpenQuestionViewModel(question);

            var changeTracker = new PropertyChangeTracker(viewModel);
            viewModel.ResponseText = "1234567890";

            Assert.IsTrue(changeTracker.ChangedProperties.Contains("ResponseText"));
        }

        // validate
    }
}
