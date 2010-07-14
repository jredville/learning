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
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using BasicMVVM.Infrastructure;
using BasicMVVM.Infrastructure.ViewModels;
using BasicMVVM.Model;

namespace BasicMVVM.ViewModels
{
    /// <summary>
    /// View model for a questionnaire.
    /// </summary>
    /// <remarks>
    /// This view model is the parent for the view models representing each question in the questionnaire.
    /// <para/>
    /// This model wraps properties from the underlying model (Name and Age) and properties that represent state for the view
    /// (UnansweredQuestions, CurrentState, and CanSubmit)
    /// </remarks>
    public class QuestionnaireViewModel : ViewModel
    {
        private readonly IQuestionnaireService questionnaireService;

        private QuestionnaireTemplate questionnaireTemplate;
        private Questionnaire questionnaire;
        private string currentState;

        public QuestionnaireViewModel()
            : this(new QuestionnaireService())
        {
        }

        public QuestionnaireViewModel(IQuestionnaireService questionnaireService)
        {
            this.questionnaireService = questionnaireService;
            this.Questions = new ObservableCollection<QuestionViewModel>();
            this.currentState = "Loading";

            this.questionnaireService.BeginGetQuestionnaireTemplate(
                ar =>
                {
                    this.questionnaireTemplate = this.questionnaireService.EndGetQuestionnaireTemplate(ar);
                    this.CreateNewQuestionnaire();
                    this.CurrentState = "Normal";
                },
                this);
        }

        public ObservableCollection<QuestionViewModel> Questions { get; private set; }

        public string Name
        {
            get
            {
                if (this.questionnaire == null)
                {
                    return null;
                }

                return this.questionnaire.Name;
            }

            set
            {
                this.questionnaire.Name = value;
                this.ValidateName();
                this.RaisePropertyChanged(() => this.CanSubmit);
            }
        }

        public string Age
        {
            get
            {
                if (this.questionnaire == null)
                {
                    return null;
                }

                return this.questionnaire.Age.HasValue ? this.questionnaire.Age.ToString() : null;
            }

            set
            {
                this.ValidateAndSetAge(value);
                this.RaisePropertyChanged(() => this.CanSubmit);
            }
        }

        public string CurrentState
        {
            get
            {
                return this.currentState;
            }

            set
            {
                if (this.currentState == value)
                {
                    return;
                }

                this.currentState = value;
                this.RaisePropertyChanged(() => this.CurrentState);
            }
        }

        public bool CanSubmit
        {
            get
            {
                return this.questionnaire != null && this.Questions.All(q => q.ResponseComplete) && this.questionnaire.IsValid;
            }
        }

        public int TotalQuestions
        {
            get { return this.Questions.Count(); }
        }

        public int UnansweredQuestions
        {
            get { return this.Questions.Where(q => !q.ResponseComplete).Count(); }
        }

        /// <summary>
        /// Submits the current questionnaire through the model.
        /// </summary>
        /// <remarks>
        /// This method updates the current state to "Submitting", which causes the view to 
        /// provide some kind of feedback, and submits the questionnaire.
        /// </remarks>
        public void Submit()
        {
            this.CurrentState = "Submitting";

            this.questionnaireService.BeginSubmitResponses(
                this.questionnaire,
                result =>
                {
                    this.questionnaireService.EndSubmitResponses(result);
                    this.UpdateSubmitStatus();
                },
                null);
        }

        public void Reset()
        {
            this.CreateNewQuestionnaire();
        }

        private void UpdateSubmitStatus()
        {
            var dispatcher = Deployment.Current.Dispatcher;
            if (!dispatcher.CheckAccess())
            {
                dispatcher.BeginInvoke(this.UpdateSubmitStatus);
                return;
            }

            this.CurrentState = "Normal";
            this.CreateNewQuestionnaire();
        }

        private void ValidateName()
        {
            // Gets the domain validation from the model through the IDataErrorInfo.Item indexer
            var nameError = this.questionnaire["Name"];

            if (!string.IsNullOrEmpty(nameError))
            {
                this.SetError(() => this.Name, nameError);
            }
            else
            {
                this.ClearErrors(() => this.Name);
            }
        }

        private void ValidateAndSetAge(string age)
        {
            int convertedValue = 0;
            if (!string.IsNullOrEmpty(age) && !age.TryConvertValue(out convertedValue))
            {
                this.SetError(() => this.Age, "Cannot convert the input to a number.");
                this.questionnaire.Age = null;
                return;
            }

            this.questionnaire.Age = string.IsNullOrEmpty(age) ? (int?)null : convertedValue;

            this.ValidateAge();
        }

        private void ValidateAge()
        {
            // Gets the domain validation from the model through the IDataErrorInfo.Item indexer
            var ageError = this.questionnaire["Age"];

            if (!string.IsNullOrEmpty(ageError))
            {
                this.SetError(() => this.Age, ageError);
            }
            else
            {
                this.ClearErrors(() => this.Age);
            }
        }

        private void CreateNewQuestionnaire()
        {
            this.questionnaire = new Questionnaire(this.questionnaireTemplate);

            this.Questions.Clear();
            foreach (var question in this.questionnaire.Questions)
            {
                this.Questions.Add(this.CreateQuestionViewModel(question));
            }

            // Raise change notification for all properties
            // see http://msdn.microsoft.com/en-us/library/system.componentmodel.inotifypropertychanged.propertychanged.aspx for details
            this.RaisePropertyChanged(string.Empty);

            // Clear errors
            this.ClearErrors(() => this.Name);
            this.ClearErrors(() => this.Age);
        }

        private QuestionViewModel CreateQuestionViewModel(Question question)
        {
            QuestionViewModel vm = null;
            var questionTemplate = question.QuestionTemplate;

            if (questionTemplate is OpenQuestionTemplate)
            {
                vm = new OpenQuestionViewModel(question);
            }
            else if (questionTemplate is MultipleSelectionQuestionTemplate)
            {
                vm = new MultipleSelectionQuestionViewModel(question);
            }
            else if (questionTemplate is NumericQuestionTemplate)
            {
                vm = new NumericQuestionViewModel(question);
            }
            else
            {
                throw new ArgumentException("Unknown question type.", "question");
            }

            vm.ResponseChanged += this.OnQuestionResponseChanged;

            return vm;
        }

        private void OnQuestionResponseChanged(object sender, EventArgs args)
        {
            this.RaisePropertyChanged(() => this.UnansweredQuestions);
            this.RaisePropertyChanged(() => this.CanSubmit);
        }
    }
}
