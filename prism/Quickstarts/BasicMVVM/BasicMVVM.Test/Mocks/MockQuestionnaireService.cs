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
using BasicMVVM.Model;

namespace BasicMVVM.Test.Mocks
{
    public class MockQuestionnaireService : IQuestionnaireService
    {
        public Action<AsyncResult<QuestionnaireTemplate>> HandleBeginGetQuestionnaireTemplate { get; set; }
        public Action<AsyncResult<object>> HandleBeginSubmitResponses { get; set; }

        public IAsyncResult BeginGetQuestionnaireTemplate(AsyncCallback callback, object asyncState)
        {
            var result = new AsyncResult<QuestionnaireTemplate>(callback, asyncState);
            HandleBeginGetQuestionnaireTemplate(result);

            return result;
        }

        public QuestionnaireTemplate EndGetQuestionnaireTemplate(IAsyncResult asyncResult)
        {
            var result = (AsyncResult<QuestionnaireTemplate>)asyncResult;
            AsyncResult<QuestionnaireTemplate>.End(result);

            return result.Result;
        }

        public void ProceedGetQuestionnaireTemplate(AsyncResult<QuestionnaireTemplate> result, QuestionnaireTemplate questionnaireTemplate)
        {
            result.SetComplete(questionnaireTemplate, false);
        }

        public IAsyncResult BeginSubmitResponses(Questionnaire questionnaire, AsyncCallback callback, object asyncState)
        {
            var result = new AsyncResult<object>(callback, asyncState);
            HandleBeginSubmitResponses(result);

            return result;
        }

        public virtual void EndSubmitResponses(IAsyncResult asyncResult)
        {
            AsyncResult<object>.End((AsyncResult<object>)asyncResult);
        }

        public void ProceedSubmitResponses(AsyncResult<object> result)
        {
            result.SetComplete(null, false);
        }
    }
}
