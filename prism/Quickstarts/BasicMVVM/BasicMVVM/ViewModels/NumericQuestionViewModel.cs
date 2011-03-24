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
using System.Globalization;
using BasicMVVM.Infrastructure;
using BasicMVVM.Model;

namespace BasicMVVM.ViewModels
{
    /// <summary>
    /// View model for numeric questions.
    /// </summary>
    /// <remarks>
    /// Numeric questions expect ints within a given range as answers, but the View for these questions uses a
    /// TextBox to gather the input from the user, which requires converting the string from the TextBox into an int.
    /// This view model could have an int-typed property to expose the response and let the binding for the Text property
    /// on the TextBox perform the conversion. This, however, would cause the view model to be unaware of such an error, 
    /// unless it is notified of it somehow, and wouldn't let it influence the error message.
    /// Instead, the view model has a string-typed property and performs the conversion to an int itself before feeding
    /// the converted value to the model; if the conversion fails then the corresponding error is notified to the view 
    /// through the INotifyDataErrorInfo interface.
    /// </remarks>
    public class NumericQuestionViewModel : QuestionViewModel
    {
        private readonly NumericQuestionTemplate questionTemplate;

        public NumericQuestionViewModel(Question question)
            : base(question)
        {
            if (question == null)
            {
                throw new ArgumentNullException("question");
            }

            this.questionTemplate = question.QuestionTemplate as NumericQuestionTemplate;
            if (this.questionTemplate == null)
            {
                throw new ArgumentException("Question is not a numeric question.", "question");
            }
        }

        public string ResponseValue
        {
            get
            {
                return this.Question.Response != null ? this.Question.Response.ToString() : null;
            }

            set
            {
                int convertedValue = 0;
                if (!value.TryConvertValue(out convertedValue))
                {
                    // Failed to convert the value. Notify the error.
                    var hadErrors = this.HasErrors;
                    this.SetError(() => this.ResponseValue, "Cannot convert the input to a number.");

                    // If going from a valid to an invalid state, notify that the response has changed too.
                    if (!hadErrors)
                    {
                        this.OnResponseChanged();
                    }
                }
                else
                {
                    // Set the converted value as the response and, by doing this, force the domain validation to be
                    // triggered and notified if necessary.
                    this.SetResponse(convertedValue);
                }
            }
        }

        /// <summary>
        /// Initiates the domain validation defined by the model, and feeds the result in the implementation for
        /// INotifyDataErrorInfo.
        /// </summary>
        protected override void ValidateResponse()
        {
            if (!this.questionTemplate.IsValid(this.Question))
            {
                this.SetError(() => this.ResponseValue, string.Format(CultureInfo.CurrentCulture, "The value needs to be a positive integer less than or equal to {0}.", this.questionTemplate.MaxValue));
            }
            else
            {
                this.ClearErrors(() => this.ResponseValue);
            }            
        }
    }
}
