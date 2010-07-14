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
using BasicMVVM.Infrastructure.ViewModels;

namespace BasicMVVM.ViewModels
{
    public abstract class QuestionViewModel : ViewModel
    {
        private readonly Question question;

        public QuestionViewModel(Question question)
        {
            this.question = question;
        }

        public event EventHandler<EventArgs> ResponseChanged;

        public string QuestionText
        {
            get { return this.question.QuestionText; }
        }

        public virtual bool ResponseComplete
        {
            get { return !this.HasErrors && this.question.ResponseComplete; }
        }

        protected Question Question
        {
            get { return this.question; }
        }

        protected void SetResponse(object value)
        {
            this.question.Response = value;
            this.ValidateResponse();
            this.OnResponseChanged();
        }

        protected virtual void ValidateResponse()
        {
        }

        protected void OnResponseChanged()
        {
            var handler = this.ResponseChanged;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }
    }
}
