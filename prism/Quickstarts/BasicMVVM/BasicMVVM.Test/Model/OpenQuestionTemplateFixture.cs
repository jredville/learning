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
using Microsoft.Silverlight.Testing;

namespace BasicMVVM.Test.Model
{
    [TestClass]
    public class OpenQuestionTemplateFixture
    {
        [TestMethod]
        public void WhenValidatingANullAnswer_ThenReturnsFalse()
        {
            var template = new OpenQuestionTemplate { MaxLength = 10 };
            var question = new Question(template);

            question.Response = null;

            Assert.IsFalse(template.IsValid(question));
        }

        [TestMethod]
        public void WhenValidatingANonStringAnswer_ThenReturnsFalse()
        {
            var template = new OpenQuestionTemplate { MaxLength = 10 };
            var question = new Question(template);

            question.Response = 4;

            Assert.IsFalse(template.IsValid(question));
        }

        [TestMethod]
        public void WhenValidatingAnEmptyString_ThenReturnsFalse()
        {
            var template = new OpenQuestionTemplate { MaxLength = 10 };
            var question = new Question(template);

            question.Response = "";

            Assert.IsFalse(template.IsValid(question));
        }

        [TestMethod]
        public void WhenValidatingANonEmptyStringLongerThanMaxLength_ThenReturnsFalse()
        {
            var template = new OpenQuestionTemplate { MaxLength = 10 };
            var question = new Question(template);

            question.Response = "1234567890ABCDEF";

            Assert.IsFalse(template.IsValid(question));
        }

        [TestMethod]
        public void WhenValidatingANonEmptyStringShorterThanMaxLength_ThenReturnsTrue()
        {
            var template = new OpenQuestionTemplate { MaxLength = 10 };
            var question = new Question(template);

            question.Response = "123456789";

            Assert.IsTrue(template.IsValid(question));
        }
    }
}
