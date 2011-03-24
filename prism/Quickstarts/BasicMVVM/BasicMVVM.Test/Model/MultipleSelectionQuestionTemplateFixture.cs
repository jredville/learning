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
    public class MultipleSelectionQuestionTemplateFixture
    {
        [TestMethod]
        public void WhenValidatingANullAnswer_ThenReturnsFalse()
        {
            var template = new MultipleSelectionQuestionTemplate { MaxSelections = 2, Range = { "a", "b", "c" } };
            var question = new Question(template);

            question.Response = null;

            Assert.IsFalse(template.IsValid(question));
        }

        [TestMethod]
        public void WhenValidatingANonEnumerableAnswer_ThenReturnsFalse()
        {
            var template = new MultipleSelectionQuestionTemplate { MaxSelections = 2, Range = { "a", "b", "c" } };
            var question = new Question(template);

            question.Response = 10;

            Assert.IsFalse(template.IsValid(question));
        }

        [TestMethod]
        public void WhenValidatingAnEnumerableAnswerWithMoreElementsThanMaxValue_ThenReturnsFalse()
        {
            var template = new MultipleSelectionQuestionTemplate { MaxSelections = 2, Range = { "a", "b", "c" } };
            var question = new Question(template);

            question.Response = new[] { "a", "b", "c" };

            Assert.IsFalse(template.IsValid(question));
        }

        [TestMethod]
        public void WhenValidatingAnEnumerableAnswerWithElementsNotInTheRange_ThenReturnsFalse()
        {
            var template = new MultipleSelectionQuestionTemplate { MaxSelections = 2, Range = { "a", "b", "c" } };
            var question = new Question(template);

            question.Response = new[] { "a", "z" };

            Assert.IsFalse(template.IsValid(question));
        }

        [TestMethod]
        public void WhenValidatingAnEnumerableAnswerWithElementsFromTheRangeWithCountEqualOrLessThanMaxSelections_ThenReturnsTrue()
        {
            var template = new MultipleSelectionQuestionTemplate { MaxSelections = 2, Range = { "a", "b", "c" } };
            var question = new Question(template);

            question.Response = new[] { "a" };

            Assert.IsTrue(template.IsValid(question));
        }
    }
}
