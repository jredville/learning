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
namespace BasicMVVM.Test.ViewModels
{
    using System;
    using System.ComponentModel;
    using System.Linq;
    using BasicMVVM.Model;
    using BasicMVVM.Test.Mocks;
    using BasicMVVM.Test.ViewModels.Utility;
    using BasicMVVM.ViewModels;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class QuestionnaireViewModelFixture
    {
        [TestMethod]
        public void WhenQuestionnaireIsCreatedWithService_ThenRetrievesQuestionnaireFromService()
        {
            AsyncResult<QuestionnaireTemplate> result = null;
            var service = new MockQuestionnaireService { HandleBeginGetQuestionnaireTemplate = ar => result = ar };

            var viewModel = new QuestionnaireViewModel(service);
            var changeTracker = new PropertyChangeTracker(viewModel);

            Assert.IsNotNull(result);
            Assert.AreEqual("Loading", viewModel.CurrentState);
            Assert.AreEqual(0, changeTracker.ChangedProperties.Count());

            service.ProceedGetQuestionnaireTemplate(result, CreateQuestionnaireTemplate());

            // Assertions
            Assert.IsTrue(changeTracker.ChangedProperties.Contains(""));
            Assert.AreEqual("Normal", viewModel.CurrentState);
            Assert.IsTrue(changeTracker.ChangedProperties.Contains("CurrentState"));
            CollectionAssert.AreEqual(
                new[] { "Enter your name", "Enter your address" },
                viewModel.Questions.Select(q => q.QuestionText).ToArray());
        }

        [TestMethod]
        public void WhenQuestionnaireIsCompleted_ThenCanSubmitReturnsTrue()
        {
            AsyncResult<QuestionnaireTemplate> result = null;
            var service = new MockQuestionnaireService { HandleBeginGetQuestionnaireTemplate = ar => result = ar };

            var viewModel = new QuestionnaireViewModel(service);
            service.ProceedGetQuestionnaireTemplate(result, CreateQuestionnaireTemplate());

            viewModel.Name = "QuestionnaireName";
            viewModel.Age = "5";
            AnswerAllQuestions(viewModel);

            // Assertions
            Assert.IsTrue(viewModel.CanSubmit);
        }

        [TestMethod]
        public void WhenQuestionnaireIsNotCompleted_ThenCanSubmitReturnsFalse()
        {
            AsyncResult<QuestionnaireTemplate> result = null;
            var service = new MockQuestionnaireService { HandleBeginGetQuestionnaireTemplate = ar => result = ar };
            var viewModel = new QuestionnaireViewModel(service);

            service.ProceedGetQuestionnaireTemplate(result, CreateQuestionnaireTemplate());

            AnswerAllQuestions(viewModel);

            // Assertions
            Assert.IsFalse(viewModel.CanSubmit);
        }

        [TestMethod]
        public void WhenQuestionnaireIsCreated_ThenAllQuestionsAreUnanswered()
        {
            AsyncResult<QuestionnaireTemplate> result = null;
            var service = new MockQuestionnaireService { HandleBeginGetQuestionnaireTemplate = ar => result = ar };
            var viewModel = new QuestionnaireViewModel(service);

            service.ProceedGetQuestionnaireTemplate(result, CreateQuestionnaireTemplate());

            // Assertions
            Assert.AreEqual(2, viewModel.TotalQuestions);
            Assert.AreEqual(2, viewModel.UnansweredQuestions);
        }

        [TestMethod]
        public void WhenQuestionIsAnswered_ThenAllUnansweredQuestionsAreUpdated()
        {
            AsyncResult<QuestionnaireTemplate> result = null;
            var service = new MockQuestionnaireService { HandleBeginGetQuestionnaireTemplate = ar => result = ar };
            var viewModel = new QuestionnaireViewModel(service);

            service.ProceedGetQuestionnaireTemplate(result, CreateQuestionnaireTemplate());

            var question = viewModel.Questions.First() as OpenQuestionViewModel;
            question.ResponseText = "some text";

            // Assertions
            Assert.AreEqual(2, viewModel.TotalQuestions);
            Assert.AreEqual(1, viewModel.UnansweredQuestions);
        }

        [TestMethod]
        public void WhenQuestionIsAnswered_ThenQuestionnaireModelNotifiesUpdate()
        {
            AsyncResult<QuestionnaireTemplate> result = null;
            var service = new MockQuestionnaireService { HandleBeginGetQuestionnaireTemplate = ar => result = ar };
            var viewModel = new QuestionnaireViewModel(service);

            service.ProceedGetQuestionnaireTemplate(result, CreateQuestionnaireTemplate());

            var question = viewModel.Questions.First() as OpenQuestionViewModel;
            var changeTracker = new PropertyChangeTracker(viewModel);
            question.ResponseText = "some text";

            // Assertions
            Assert.IsTrue(changeTracker.ChangedProperties.Contains("UnansweredQuestions"));
        }

        [TestMethod]
        public void WhenQuestionnaireIsSubmitted_ThenSubmitsToService()
        {
            bool submitCalled = false;
            AsyncResult<QuestionnaireTemplate> result = null;
            var service =
                new MockQuestionnaireService
                {
                    HandleBeginGetQuestionnaireTemplate = ar => result = ar,
                    HandleBeginSubmitResponses = ar => submitCalled = true
                };
            var viewModel = new QuestionnaireViewModel(service);

            service.ProceedGetQuestionnaireTemplate(result, CreateQuestionnaireTemplate());

            viewModel.Name = "Name";
            viewModel.Age = "21";
            AnswerAllQuestions(viewModel);
            viewModel.Submit();

            Assert.IsTrue(submitCalled);
        }

        [TestMethod]
        public void WhenCurrentStateIsModified_ThenViewModelNotifiesUpdate()
        {
            AsyncResult<QuestionnaireTemplate> result = null;
            var service = new MockQuestionnaireService { HandleBeginGetQuestionnaireTemplate = ar => result = ar };
            var viewModel = new QuestionnaireViewModel(service);

            service.ProceedGetQuestionnaireTemplate(result, CreateQuestionnaireTemplate());

            var changeTracker = new PropertyChangeTracker(viewModel);

            viewModel.CurrentState = "newState";

            // Assertions
            Assert.IsTrue(changeTracker.ChangedProperties.Contains("CurrentState"));
        }

        [TestMethod]
        public void WhenServiceIsDoneSubmitting_ThenStateIsSetToNormal()
        {
            AsyncResult<QuestionnaireTemplate> getResult = null;
            AsyncResult<object> submitResult = null;
            var service =
                new MockQuestionnaireService
                {
                    HandleBeginGetQuestionnaireTemplate = ar => getResult = ar,
                    HandleBeginSubmitResponses = ar => submitResult = ar
                };
            var viewModel = new QuestionnaireViewModel(service);

            service.ProceedGetQuestionnaireTemplate(getResult, CreateQuestionnaireTemplate());

            viewModel.Submit();
            Assert.AreEqual("Submitting", viewModel.CurrentState);

            service.ProceedSubmitResponses(submitResult);

            Assert.AreEqual("Normal", viewModel.CurrentState);
        }

        [TestMethod]
        public void AfterSubmitting_ANewQuestionnaireIsCreated()
        {
            QuestionViewModel[] originalQuestions = null;
            AsyncResult<QuestionnaireTemplate> getResult = null;
            AsyncResult<object> submitResult = null;
            var service =
                new MockQuestionnaireService
                {
                    HandleBeginGetQuestionnaireTemplate = ar => getResult = ar,
                    HandleBeginSubmitResponses = ar => submitResult = ar
                };
            var viewModel = new QuestionnaireViewModel(service);

            service.ProceedGetQuestionnaireTemplate(getResult, CreateQuestionnaireTemplate());

            originalQuestions = viewModel.Questions.ToArray();

            viewModel.Name = "TestName";
            viewModel.Age = "14";
            AnswerAllQuestions(viewModel);

            viewModel.Submit();

            service.ProceedSubmitResponses(submitResult);

            CollectionAssert.AreNotEquivalent(originalQuestions, viewModel.Questions.ToArray());
        }

        [TestMethod]
        public void AfterResetting_ThenANewQuestionnaireIsCreated()
        {
            QuestionViewModel[] originalQuestions = null;
            AsyncResult<QuestionnaireTemplate> getResult = null;
            var service =
                new MockQuestionnaireService
                {
                    HandleBeginGetQuestionnaireTemplate = ar => getResult = ar,
                };
            var viewModel = new QuestionnaireViewModel(service);

            service.ProceedGetQuestionnaireTemplate(getResult, CreateQuestionnaireTemplate());

            originalQuestions = viewModel.Questions.ToArray();

            viewModel.Name = "TestName";
            viewModel.Age = "14";
            AnswerAllQuestions(viewModel);

            viewModel.Reset();

            CollectionAssert.AreNotEquivalent(originalQuestions, viewModel.Questions.ToArray());
        }

        [TestMethod]
        public void AfterResetting_ThenErrorsAreCleared()
        {
            AsyncResult<QuestionnaireTemplate> getResult = null;
            var service = new MockQuestionnaireService { HandleBeginGetQuestionnaireTemplate = ar => getResult = ar };
            var viewModel = new QuestionnaireViewModel(service);
            var viewModelIndei = viewModel as INotifyDataErrorInfo;

            service.ProceedGetQuestionnaireTemplate(getResult, CreateQuestionnaireTemplate());

            viewModel.Age = "aa";

            // Assertions
            Assert.IsTrue(viewModelIndei.HasErrors);
            Assert.IsTrue(viewModelIndei.GetErrors("Age").OfType<object>().Any());

            bool errorsChanged = false;
            ((INotifyDataErrorInfo)viewModel).ErrorsChanged += (s, a) => errorsChanged = true;
            viewModel.Reset();

            Assert.IsFalse(viewModelIndei.HasErrors);
            Assert.IsNull(viewModelIndei.GetErrors("Age"));
            Assert.IsTrue(errorsChanged);
        }

        [TestMethod]
        public void AfterSubmittingQuestionnaire_ThenChangeDataRelatedNotificationsAreFired()
        {
            AsyncResult<QuestionnaireTemplate> getResult = null;
            AsyncResult<object> submitResult = null;
            var service =
                new MockQuestionnaireService
                {
                    HandleBeginGetQuestionnaireTemplate = ar => getResult = ar,
                    HandleBeginSubmitResponses = ar => submitResult = ar
                };
            var viewModel = new QuestionnaireViewModel(service);

            service.ProceedGetQuestionnaireTemplate(getResult, CreateQuestionnaireTemplate());

            var tracker = new PropertyChangeTracker(viewModel);
            viewModel.Name = "TestName";
            viewModel.Age = "111";
            AnswerAllQuestions(viewModel);

            viewModel.Submit();

            service.ProceedSubmitResponses(submitResult);

            Assert.IsTrue(tracker.ChangedProperties.Contains(""));
        }

        [TestMethod]
        public void WhenQuestionnaireHasOpenTextQuestion_ThenTheMatchingViewModelIsCreated()
        {
            AsyncResult<QuestionnaireTemplate> getResult = null;
            var service = new MockQuestionnaireService { HandleBeginGetQuestionnaireTemplate = ar => getResult = ar };
            var viewModel = new QuestionnaireViewModel(service);

            service.ProceedGetQuestionnaireTemplate(
                getResult,
                new QuestionnaireTemplate { Questions = { new OpenQuestionTemplate { QuestionText = "Open" } } });

            // Assertions
            Assert.IsInstanceOfType(viewModel.Questions[0], typeof(OpenQuestionViewModel));
        }

        [TestMethod]
        public void WhenQuestionnaireHasMultipleSelectionQuestion_ThenTheMatchingViewModelIsCreated()
        {
            AsyncResult<QuestionnaireTemplate> getResult = null;
            var service = new MockQuestionnaireService { HandleBeginGetQuestionnaireTemplate = ar => getResult = ar };
            var viewModel = new QuestionnaireViewModel(service);

            service.ProceedGetQuestionnaireTemplate(
                getResult,
                new QuestionnaireTemplate { Questions = { new MultipleSelectionQuestionTemplate { QuestionText = "Multiple selection" } } });

            // Assertions
            Assert.IsInstanceOfType(viewModel.Questions[0], typeof(MultipleSelectionQuestionViewModel));
        }

        [TestMethod]
        public void WhenQuestionnaireHasNumericQuestion_ThenTheMatchingViewModelIsCreated()
        {
            AsyncResult<QuestionnaireTemplate> getResult = null;
            var service = new MockQuestionnaireService { HandleBeginGetQuestionnaireTemplate = ar => getResult = ar };
            var viewModel = new QuestionnaireViewModel(service);

            service.ProceedGetQuestionnaireTemplate(
                getResult,
                new QuestionnaireTemplate { Questions = { new NumericQuestionTemplate { QuestionText = "Numeric" } } });

            // Assertions
            Assert.IsInstanceOfType(viewModel.Questions[0], typeof(NumericQuestionViewModel));
        }

        [TestMethod]
        public void WhenQuestionnaireHasUnknownQuestion_ThenAnExceptionIsThrown()
        {
            AsyncResult<QuestionnaireTemplate> getResult = null;
            var service = new MockQuestionnaireService { HandleBeginGetQuestionnaireTemplate = ar => getResult = ar };
            var viewModel = new QuestionnaireViewModel(service);

            try
            {
                service.ProceedGetQuestionnaireTemplate(
                    getResult,
                    new QuestionnaireTemplate { Questions = { new MockQuestionTemplate { QuestionText = "unknown" } } });
                Assert.Fail("Expected ArgumentException");
            }
            catch (ArgumentException)
            {
                // expected exception
            }
        }

        [TestMethod]
        public void WhenQuestionHasFormattingErrorAfterHavingValidResponse_ThenViewModelCannotBeSubmitted()
        {
            AsyncResult<QuestionnaireTemplate> getResult = null;
            var service = new MockQuestionnaireService { HandleBeginGetQuestionnaireTemplate = ar => getResult = ar };
            var viewModel = new QuestionnaireViewModel(service);

            service.ProceedGetQuestionnaireTemplate(
                getResult,
                new QuestionnaireTemplate { Questions = { new NumericQuestionTemplate { QuestionText = "unknown" } } });

            var questionViewModel = ((NumericQuestionViewModel)viewModel.Questions[0]);

            viewModel.Name = "name";
            viewModel.Age = "5";
            questionViewModel.ResponseValue = "100";

            Assert.IsTrue(viewModel.CanSubmit);

            questionViewModel.ResponseValue = "abc";

            Assert.IsFalse(viewModel.CanSubmit);
        }

        [TestMethod]
        public void WhenViewModelHasMaxNameLength_ThenChangingNameConformsWithINotifyDataErrorInfo()
        {
            AsyncResult<QuestionnaireTemplate> getResult = null;
            var service = new MockQuestionnaireService { HandleBeginGetQuestionnaireTemplate = ar => getResult = ar };
            var viewModel = new QuestionnaireViewModel(service);

            service.ProceedGetQuestionnaireTemplate(
                getResult,
                new QuestionnaireTemplate
                {
                    Questions = { new NumericQuestionTemplate { QuestionText = "unknown" } },
                    NameMaxLength = 3
                });

            viewModel.Age = "5";

            new NotifyDataErrorInfoTestHelper<QuestionnaireViewModel, string>(
                viewModel,
                vm => vm.Name)
                .ValidateAllPropertyChanges("012", "01", "01234", "012345");
        }

        [TestMethod]
        public void WhenViewModelHasMinAge_ThenChangingAgeConformsWithINotifyDataErrorInfo()
        {
            AsyncResult<QuestionnaireTemplate> getResult = null;
            var service = new MockQuestionnaireService { HandleBeginGetQuestionnaireTemplate = ar => getResult = ar };
            var viewModel = new QuestionnaireViewModel(service);

            service.ProceedGetQuestionnaireTemplate(
                getResult,
                new QuestionnaireTemplate
                {
                    Questions = { new NumericQuestionTemplate { QuestionText = "unknown" } },
                    MinAge = 10
                });

            viewModel.Name = "AName";

            new NotifyDataErrorInfoTestHelper<QuestionnaireViewModel, string>(
                viewModel,
                vm => vm.Age)
                .ValidateAllPropertyChanges("15", "20", "1", "2");
        }

        [TestMethod]
        public void WhenViewModelHasMaxAge_ThenChangingAgeConformsWithINotifyDataErrorInfo()
        {
            AsyncResult<QuestionnaireTemplate> getResult = null;
            var service = new MockQuestionnaireService { HandleBeginGetQuestionnaireTemplate = ar => getResult = ar };
            var viewModel = new QuestionnaireViewModel(service);

            service.ProceedGetQuestionnaireTemplate(
                getResult,
                new QuestionnaireTemplate
                {
                    Questions = { new NumericQuestionTemplate { QuestionText = "unknown" } },
                    MaxAge = 30
                });

            viewModel.Name = "AName";

            new NotifyDataErrorInfoTestHelper<QuestionnaireViewModel, string>(
                viewModel,
                vm => vm.Age)
                .ValidateAllPropertyChanges("15", "20", "35", "40");
        }

        [TestMethod]
        public void WhenSettingAgeWithInvalidFormats_ThenChangingAgeConformsWithINotifyDataErrorInfo()
        {
            AsyncResult<QuestionnaireTemplate> getResult = null;
            var service = new MockQuestionnaireService { HandleBeginGetQuestionnaireTemplate = ar => getResult = ar };
            var viewModel = new QuestionnaireViewModel(service);

            service.ProceedGetQuestionnaireTemplate(
                getResult,
                new QuestionnaireTemplate
                {
                    Questions = { new NumericQuestionTemplate { QuestionText = "unknown" } }
                });

            viewModel.Name = "AName";

            new NotifyDataErrorInfoTestHelper<QuestionnaireViewModel, string>(
                viewModel,
                vm => vm.Age)
                .ValidateAllPropertyChanges("15", "20", "AnInvalidFormatForAge", "anotherInvalidFormatForAge");
        }

        [TestMethod]
        public void WhenSettingAgeWithInvalidFormatsAndAgeConstrains_ThenChangingAgeConformsWithINotifyDataErrorInfo()
        {
            AsyncResult<QuestionnaireTemplate> getResult = null;
            var service = new MockQuestionnaireService { HandleBeginGetQuestionnaireTemplate = ar => getResult = ar };
            var viewModel = new QuestionnaireViewModel(service);

            service.ProceedGetQuestionnaireTemplate(
                getResult,
                new QuestionnaireTemplate
                {
                    Questions = { new NumericQuestionTemplate { QuestionText = "unknown" } },
                    MaxAge = 30
                });

            viewModel.Name = "AName";

            var helper = new NotifyDataErrorInfoTestHelper<QuestionnaireViewModel, string>(
                viewModel,
                vm => vm.Age);

            // invalid age --> invalid format
            viewModel.Age = "35";
            helper.ValidatePropertyChange("AnInvalidFormatForAge", NotifyDataErrorInfoBehavior.FiresErrorsChanged | NotifyDataErrorInfoBehavior.HasErrors | NotifyDataErrorInfoBehavior.HasErrorsForProperty);

            // invalid format --> invalid age
            viewModel.Age = "AnInvalidFormatForAge";
            helper.ValidatePropertyChange("46", NotifyDataErrorInfoBehavior.FiresErrorsChanged | NotifyDataErrorInfoBehavior.HasErrors | NotifyDataErrorInfoBehavior.HasErrorsForProperty);
        }

        [TestMethod]
        public void WhenGettingCalculatedPropertiesBeforeGettingAQuestionnaire_ThenReturnsDefaultValues()
        {
            var service = new Mock<IQuestionnaireService>();
            service
                .Setup(svc => svc.BeginGetQuestionnaireTemplate(It.IsAny<AsyncCallback>(), It.IsAny<object>()))
                .Returns(new AsyncResult<QuestionnaireTemplate>(null, null));

            var viewModel = new QuestionnaireViewModel(service.Object);

            Assert.AreEqual(0, viewModel.TotalQuestions);
            Assert.AreEqual(0, viewModel.UnansweredQuestions);
            Assert.IsFalse(viewModel.CanSubmit);
            Assert.AreEqual(null, viewModel.Name);
            Assert.AreEqual(null, viewModel.Age);
        }

        private static Questionnaire CreateNonAnsweredQuestionnaire()
        {
            var questionnaireTemplate = CreateQuestionnaireTemplate();

            var questionnaire = new Questionnaire(questionnaireTemplate);

            return questionnaire;
        }

        private static QuestionnaireTemplate CreateQuestionnaireTemplate()
        {
            var questionnaireTemplate = new QuestionnaireTemplate()
            {
                Questions = 
                        { 
                            new OpenQuestionTemplate() { QuestionText = "Enter your name" },
                            new OpenQuestionTemplate() { QuestionText = "Enter your address"}
                        }
            };
            return questionnaireTemplate;
        }

        private static void AnswerAllQuestions(QuestionnaireViewModel viewModel)
        {
            foreach (OpenQuestionViewModel questionViewModel in viewModel.Questions)
            {
                questionViewModel.ResponseText = "response";
            }
        }

        private static bool AreSame(IAsyncResult expectedResult, IAsyncResult result)
        {
            return expectedResult == result;
        }
    }
}
