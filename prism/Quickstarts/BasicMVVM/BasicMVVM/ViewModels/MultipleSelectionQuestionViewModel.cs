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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using BasicMVVM.Infrastructure.ViewModels;
using BasicMVVM.Model;

namespace BasicMVVM.ViewModels
{
    /// <summary>
    /// View model for multiple selection questions.
    /// </summary>
    /// <remarks>
    /// The model for a multiple selection question expects a list of selections as its answer, but the desired 
    /// View for this question consists of a series of check boxes, one for each option. In order to support this
    /// UI, this view model is a hierarchical view model, exposing a collection of child view models representing 
    /// each option. However, rather than mapping the checked state on each child view model to presence of an option
    /// into the list of selections, the view model exposes the list of selections and expects the view to 
    /// populate it appropriately, which the view does by using the 
    /// <see cref="BasicMVVM.Infrastructure.Behaviors.SynchronizeSelectedItems">SynchronizeSelectedItems</see> class.
    /// </remarks>
    public class MultipleSelectionQuestionViewModel : QuestionViewModel
    {
        private readonly IEnumerable<SelectionViewModel> range;
        private readonly int? maxSelections;
        private readonly ObservableCollection<string> selections;

        public MultipleSelectionQuestionViewModel(Question question)
            : base(question)
        {
            if (question == null)
            {
                throw new ArgumentNullException("question");
            }

            var questionTemplate = question.QuestionTemplate as MultipleSelectionQuestionTemplate;
            if (questionTemplate == null)
            {
                throw new ArgumentException("Not a multiple choice question.", "question");
            }

            this.range = new List<SelectionViewModel>(
                    questionTemplate.Range.Select(
                        r => this.CreateSelectionViewModel(r)));

            this.maxSelections = questionTemplate.MaxSelections;
            this.selections = new ObservableCollection<string>();
            this.selections.CollectionChanged += this.OnSelectionsChanged;
            this.SetResponse(this.selections);
        }

        /// <summary>
        /// Gets the child view models enumerable.
        /// </summary>
        public IEnumerable<SelectionViewModel> Range
        {
            get { return this.range; }
        }

        /// <summary>
        /// Gets the list of selections.
        /// </summary>
        public IList<string> Selections
        {
            get { return this.selections; }
        }

        private SelectionViewModel CreateSelectionViewModel(string r)
        {
            var selectionViewModel = new SelectionViewModel { Content = r, Parent = this };

            return selectionViewModel;
        }

        /// <summary>
        /// Triggers the domain validation for this question, and notifies the response has changed.
        /// </summary>
        private void OnSelectionsChanged(object sender, EventArgs args)
        {
            this.UpdateSelectionErrors();
            this.OnResponseChanged();
        }

        /// <summary>
        /// Performs the validation as defined by the domain model.
        /// </summary>
        private void UpdateSelectionErrors()
        {
            if (!this.Question.QuestionTemplate.IsValid(this.Question))
            {
                this.SetError(null, string.Format(CultureInfo.CurrentCulture, "There can be at most {0} answers selected", this.maxSelections.Value));
            }
            else
            {
                this.ClearErrors(null);
            }
        }

        /// <summary>
        /// View model representing an individual option in a <see cref="MultipleSelectionQuestionViewModel"/>.
        /// </summary>
        /// <remarks>
        /// The main purpose of this view model is to provide a way to navigate to the parent MultipleSelectionQuestionViewModel and
        /// access its Selections collection so that the synchronizing behavior can access it. If Silverlight allowed for more 
        /// sophisticated bindings (e.g. bindings with a RelativeSource other than TemplatedParent or Self) this view model wouldn't 
        /// be necessary and a simple list with the options would suffice.
        /// </remarks>
        public class SelectionViewModel : ViewModel
        {
            private string content;

            public string Content
            {
                get
                {
                    return this.content;
                }

                set
                {
                    if (this.content == value)
                    {
                        return;
                    }

                    this.content = value;
                    this.RaisePropertyChanged(() => this.Content);
                }
            }

            public object Parent { get; set; }
        }
    }
}