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

namespace BasicMVVM.ViewModels
{
    public class OpenQuestionViewModel : QuestionViewModel
    {
        private OpenQuestionTemplate questionTemplate;

        public OpenQuestionViewModel(Question question)
            : base(question)
        {
            this.questionTemplate = question.QuestionTemplate as OpenQuestionTemplate;
            if (this.questionTemplate == null)
            {
                throw new ArgumentException("Not an open question", "question");
            }
        }

        public int MaxLength
        {
            get { return this.questionTemplate.MaxLength; }
        }

        public int AvailableLength
        {
            get
            {
                return this.MaxLength - (this.ResponseText != null ? this.ResponseText.Length : 0);
            }
        }

        public string ResponseText
        {
            get
            {
                return this.Question.Response as string;
            }

            set
            {
                this.SetResponse(value.Length == 0 ? null : value);
                RaisePropertyChanged(() => this.AvailableLength);
                RaisePropertyChanged(() => this.ResponseText);
            }
        }
    }
}
