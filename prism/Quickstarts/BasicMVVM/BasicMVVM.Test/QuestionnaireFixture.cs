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
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BasicMVVM.Test.Mocks;

namespace BasicMVVM.Test
{
    [TestClass]
    public class QuestionnaireFixture
    {
        [TestMethod]
        public void WhenRealizingATemplateThenAllQuestionTemplatesAreCopied()
        {
            var template = CreateTemplate();
            var questionnaire = new Questionnaire(template);

            Assert.AreEqual(questionnaire.Questions.Count(), template.Questions.Count());

            CollectionAssert.AreEqual(
                    new[] { "Enter your name", "Enter your address" },
                    questionnaire.Questions.Select(q => q.QuestionText).ToArray());
        }

        [TestMethod]
        public void WhenAllQuestionsAnsweredAndNameAndAgeSetThenQuestionnaireIsValid()
        {
            var template = CreateTemplate();
            var questionnaire = new Questionnaire(template);

            foreach (var question in questionnaire.Questions)
            {
                question.Response = "AnAnswer";
            }

            questionnaire.Age = 5;
            questionnaire.Name = "AName";

            Assert.IsTrue(questionnaire.IsValid);
        }

        [TestMethod]
        public void WhenAllQuestionsAnsweredAndAgeSetButNameIsNotSetThenQuestionnaireIsNotValid()
        {
            var template = CreateTemplate();
            var questionnaire = new Questionnaire(template);

            foreach (var question in questionnaire.Questions)
            {
                question.Response = "AnAnswer";
            }

            questionnaire.Age = 5;

            Assert.IsFalse(questionnaire.IsValid);
        }

        [TestMethod]
        public void WhenAllQuestionsAnsweredAndNameSetButAgeIsNotSetThenQuestionnaireIsNotValid()
        {
            var template = CreateTemplate();
            var questionnaire = new Questionnaire(template);

            foreach (var question in questionnaire.Questions)
            {
                question.Response = "AnAnswer";
            }

            questionnaire.Name = "AName";

            Assert.IsFalse(questionnaire.IsValid);
        }

        [TestMethod]
        public void WhenNameLongerThanMaxLengthThenQuestionnaireIsNotValid()
        {
            var template = CreateTemplate();
            var questionnaire = new Questionnaire(template);
            template.NameMaxLength = 2;

            foreach (var question in questionnaire.Questions)
            {
                question.Response = "AnAnswer";
            }

            questionnaire.Age = 5;
            questionnaire.Name = "AName";

            Assert.IsFalse(questionnaire.IsValid);
        }

        [TestMethod]
        public void WhenAgeLessThanMinAgeThenQuestionnaireIsNotValid()
        {
            var template = CreateTemplate();
            var questionnaire = new Questionnaire(template);
            template.MinAge = 10;

            foreach (var question in questionnaire.Questions)
            {
                question.Response = "AnAnswer";
            }

            questionnaire.Age = 5;
            questionnaire.Name = "AName";

            Assert.IsFalse(questionnaire.IsValid);
        }

        [TestMethod]
        public void WhenAgeBiggerThanMaxAgeThenQuestionnaireIsNotValid()
        {
            var template = CreateTemplate();
            var questionnaire = new Questionnaire(template);
            template.MaxAge = 10;

            foreach (var question in questionnaire.Questions)
            {
                question.Response = "AnAnswer";
            }

            questionnaire.Age = 15;
            questionnaire.Name = "AName";

            Assert.IsFalse(questionnaire.IsValid);
        }
        
        [TestMethod]
        public void WhenAgeBiggerThanMaxAgeThenItemsContainsErrorForAge()
        {
            var template = CreateTemplate();
            var questionnaire = new Questionnaire(template);
            template.MaxAge = 10;

            questionnaire.Age = 15;
            questionnaire.Name = "AName";

            Assert.IsTrue(questionnaire["Age"].Contains(template.MaxAge.ToString()));
        }

        [TestMethod]
        public void WhenAgeLessThanMinAgeThenItemsContainsErrorForAge()
        {
            var template = CreateTemplate();
            var questionnaire = new Questionnaire(template);
            template.MinAge = 10;

           questionnaire.Age = 9;
            questionnaire.Name = "AName";

            Assert.IsTrue(questionnaire["Age"].Contains(template.MinAge.ToString()));
        }

        [TestMethod]
        public void WhenAgeIsBetweenMaxAgeAndMinAgeThenItemsShouldContainNoErrorForAge()
        {
            var template = CreateTemplate();
            var questionnaire = new Questionnaire(template);
            template.MinAge = 10;
            template.MaxAge = 30;

            questionnaire.Age = 15;
            questionnaire.Name = "AName";

            Assert.IsNull(questionnaire["Age"]);
        }

        [TestMethod]
        public void WhenNameIsLongerThanMaxNameLengthThenItemsShouldContainErrorForName()
        {
            var template = CreateTemplate();
            var questionnaire = new Questionnaire(template);
            template.NameMaxLength = 4;

            questionnaire.Name = "012345";

            Assert.IsTrue(questionnaire["Name"].Contains(template.NameMaxLength.ToString()));
        }

        [TestMethod]
        public void WhenNameIsSetToNullOrEmptyThenItemsShouldContainErrorForName()
        {
            var template = CreateTemplate();
            var questionnaire = new Questionnaire(template);
            template.NameMaxLength = 4;

            questionnaire.Name = null;
            Assert.IsFalse(string.IsNullOrEmpty(questionnaire["Name"]));
            questionnaire.Name = string.Empty;
            Assert.IsFalse(string.IsNullOrEmpty(questionnaire["Name"]));
        }

        [TestMethod]
        public void ErrosShouldReturnNull()
        {
            var template = CreateTemplate();
            var questionnaire = new Questionnaire(template);
            
            Assert.IsNull(questionnaire.Error);
        }

        private static QuestionnaireTemplate CreateTemplate()
        {
            return new QuestionnaireTemplate()
                    {
                        Questions = 
                        { 
                            new MockQuestionTemplate() { QuestionText = "Enter your name" },
                            new MockQuestionTemplate() { QuestionText = "Enter your address"}
                        }
                    };
        }
    }


}
