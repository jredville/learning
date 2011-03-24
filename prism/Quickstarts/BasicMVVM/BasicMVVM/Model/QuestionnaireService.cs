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
namespace BasicMVVM.Model
{
    using System;
    using System.Threading;

    public class QuestionnaireService : IQuestionnaireService
    {
        private readonly SynchronizationContext synchronizationContext;

        public QuestionnaireService()
        {
            this.synchronizationContext = SynchronizationContext.Current ?? new SynchronizationContext();
        }

        public IAsyncResult BeginGetQuestionnaireTemplate(AsyncCallback callback, object asyncState)
        {
            var result = new AsyncResult<QuestionnaireTemplate>(callback, asyncState);

            ThreadPool.QueueUserWorkItem(
                state =>
                {
                    var questionnaireTemplate = this.DoGetQuestionnaireTemplate();
                    this.synchronizationContext.Post(ignored => result.SetComplete(questionnaireTemplate, true), null);
                });

            return result;
        }

        public QuestionnaireTemplate EndGetQuestionnaireTemplate(IAsyncResult asyncResult)
        {
            var result = (AsyncResult<QuestionnaireTemplate>)asyncResult;
            AsyncResult<QuestionnaireTemplate>.End(result);

            return result.Result;
        }

        public IAsyncResult BeginSubmitResponses(Questionnaire questionnaire, AsyncCallback callback, object asyncState)
        {
            var result = new AsyncResult<object>(callback, asyncState);

            ThreadPool.QueueUserWorkItem(
                state =>
                {
                    DoSubmitResponses(questionnaire);
                    this.synchronizationContext.Post(ignored => result.SetComplete(null, false), null);
                });

            return result;
        }

        public void EndSubmitResponses(IAsyncResult asyncResult)
        {
            AsyncResult<object>.End((AsyncResult<object>)asyncResult);
        }

        private QuestionnaireTemplate DoGetQuestionnaireTemplate()
        {
            // simulate loading time
            Thread.Sleep(1000);

            var template =
                new QuestionnaireTemplate
                {
                    NameMaxLength = 50,
                    MaxAge = 120,
                    MinAge = 16,
                    Questions =
                        {
                            new NumericQuestionTemplate 
                            { 
                                QuestionText = "How many times do you eat out per month?", MaxValue = 15 
                            },
                            new MultipleSelectionQuestionTemplate 
                            { 
                                QuestionText = "Which are your favorite entries? (Max. 2)", 
                                Range = { "Pizza", "Pasta", "Steak", "Ribs" },
                                MaxSelections = 2 
                            },
                            new OpenQuestionTemplate 
                            { 
                                QuestionText = "What appetizers would you add to the menu?", MaxLength = 250 
                            },
                            new OpenQuestionTemplate 
                            { 
                                QuestionText = "Do you have any feedback for the management?", MaxLength = 250 
                            },
                        },
                };

            return template;
        }

        private void DoSubmitResponses(Questionnaire questionnaire)
        {
            // simulate submitting time
            Thread.Sleep(1000);
        }
    }
}
