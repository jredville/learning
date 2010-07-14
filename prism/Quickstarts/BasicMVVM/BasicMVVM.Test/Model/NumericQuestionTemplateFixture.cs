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
using BasicMVVM.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BasicMVVM.Test.Model
{
    [TestClass]
    public class NumericQuestionTemplateFixture
    {
        [TestMethod]
        public void WhenValidatingANullAnswer_ThenReturnsFalse()
        {
            var template = new NumericQuestionTemplate { MaxValue = 10 };
            var question = new Question(template);

            question.Response = null;

            Assert.IsFalse(template.IsValid(question));
        }

        [TestMethod]
        public void WhenValidatingANonIntAnswer_ThenReturnsFalse()
        {
            var template = new NumericQuestionTemplate { MaxValue = 10 };
            var question = new Question(template);

            question.Response = "not an int";

            Assert.IsFalse(template.IsValid(question));
        }

        [TestMethod]
        public void WhenValidatingAnIntAnswerLargerThanMaxValue_ThenReturnsFalse()
        {
            var template = new NumericQuestionTemplate { MaxValue = 10 };
            var question = new Question(template);

            question.Response = 25;

            Assert.IsFalse(template.IsValid(question));
        }

        [TestMethod]
        public void WhenValidatingANegativeIntAnswer_ThenReturnsFalse()
        {
            var template = new NumericQuestionTemplate { MaxValue = 10 };
            var question = new Question(template);

            question.Response = -5;

            Assert.IsFalse(template.IsValid(question));
        }

        [TestMethod]
        public void WhenValidatingAPostiveIntAnswerSmallerThanMaxValue_ThenReturnsFalse()
        {
            var template = new NumericQuestionTemplate { MaxValue = 10 };
            var question = new Question(template);

            question.Response = 8;

            Assert.IsTrue(template.IsValid(question));
        }
    }
}
