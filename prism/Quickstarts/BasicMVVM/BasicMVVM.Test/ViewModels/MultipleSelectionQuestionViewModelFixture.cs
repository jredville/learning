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
using BasicMVVM.Test.Mocks;
using BasicMVVM.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.ComponentModel;

namespace BasicMVVM.Test.ViewModels
{
    [TestClass]
    public class MultipleSelectionQuestionViewModelFixture
    {
        [TestMethod]
        public void WhenCreatingAViewModelWithANullQuestionThenAnExceptionIsThrown()
        {
            try
            {
                new MultipleSelectionQuestionViewModel(null);
                Assert.Fail("should have thrown");
            }
            catch (ArgumentException)
            {
                // expected
            }
        }

        [TestMethod]
        public void WhenCreatingAViewModelWithAQuestionForADifferentTemplateThenAnExceptionIsThrown()
        {
            var question = new Question(new MockQuestionTemplate());

            try
            {
                new MultipleSelectionQuestionViewModel(question);
                Assert.Fail("should have thrown");
            }
            catch (ArgumentException)
            {
                // expected
            }
        }

        [TestMethod]
        public void WhenCreatingAViewModelThenRangeAndMaxValueAreSetAcortingToTemplateRange()
        {
            var template = new MultipleSelectionQuestionTemplate { Range = { "a", "b", "c" } };
            var question = new Question(template);
            var viewModel = new MultipleSelectionQuestionViewModel(question);

            // Assertions
            var rangeEquals = viewModel.Range.Zip(template.Range, (selectionViewModel, range) => selectionViewModel.Content == range && selectionViewModel.Parent == viewModel);
            Assert.IsTrue(rangeEquals.All(b => b));
        }

        [TestMethod]
        public void WhenGetErrorIsCalledWithLessSelectionsThanMaxThenNullIsReturned()
        {
            var question = new Question(new MultipleSelectionQuestionTemplate { MaxSelections = 2, Range = { "a", "b", "c" } });
            var viewModel = new MultipleSelectionQuestionViewModel(question);

            viewModel.Selections.Add("a");
            viewModel.Selections.Add("c");

            // Assertions
            var viewModelIndei = viewModel as INotifyDataErrorInfo;
            Assert.IsNull(viewModelIndei.GetErrors(null));
            Assert.IsFalse(viewModelIndei.HasErrors);
        }

        [TestMethod]
        public void WhenGetErrorIsCalledWithMoreSelectionsThanMaxThenErrorIsReturned()
        {
            var question = new Question(new MultipleSelectionQuestionTemplate { MaxSelections = 2, Range = { "a", "b", "c" } });
            var viewModel = new MultipleSelectionQuestionViewModel(question);

            viewModel.Selections.Add("a");
            viewModel.Selections.Add("c");
            viewModel.Selections.Add("b");

            // Assertions
            var viewModelIndei = viewModel as INotifyDataErrorInfo;
            Assert.IsNotNull(viewModelIndei.GetErrors(null));
            Assert.IsTrue(viewModelIndei.HasErrors);
        }

        [TestMethod]
        public void WhenGetErrorIsCalledWithNotSelectionPropertyThenNullIsReturned()
        {
            var viewModel = new MultipleSelectionQuestionViewModel(new Question(new MultipleSelectionQuestionTemplate { MaxSelections = 2 }));

            // Assertions
            var viewModelIndei = viewModel as INotifyDataErrorInfo;
            Assert.IsNull(viewModelIndei.GetErrors("NotSelections"));
            Assert.IsFalse(viewModelIndei.HasErrors);
        }

        [TestMethod]
        public void WhenAddingExceedingSelectionThenHasErrorsIsUpdated()
        {
            var question = new Question(new MultipleSelectionQuestionTemplate { MaxSelections = 2, Range = { "a", "b", "c" } });
            var viewModel = new MultipleSelectionQuestionViewModel(question);

            viewModel.Selections.Add("a");
            viewModel.Selections.Add("c");
            viewModel.Selections.Add("b");

            // Assertions
            var viewModelIndei = viewModel as INotifyDataErrorInfo;
            Assert.IsTrue(viewModelIndei.HasErrors);
        }

        [TestMethod]
        public void WhenRemovingExceedingSelectionThenHasErrorsIsUpdated()
        {
            var question = new Question(new MultipleSelectionQuestionTemplate { MaxSelections = 2, Range = { "a", "b", "c" } });
            var viewModel = new MultipleSelectionQuestionViewModel(question);

            viewModel.Selections.Add("a");
            viewModel.Selections.Add("c");
            viewModel.Selections.Add("b");
            viewModel.Selections.Remove("c");

            // Assertions
            var viewModelIndei = viewModel as INotifyDataErrorInfo;
            Assert.IsFalse(viewModelIndei.HasErrors);
        }

        [TestMethod]
        public void WhenAddingNewSelectionThatDoesNotExceedMaxSelectionsThenErrorsChangedIsNotFired()
        {
            var question =
                new Question(new MultipleSelectionQuestionTemplate { MaxSelections = 2, Range = { "a", "b", "c" } });
            var viewModel = new MultipleSelectionQuestionViewModel(question);
           

            bool errorChangedCalled = false;
            ((INotifyDataErrorInfo)viewModel).ErrorsChanged += (s, e) => { errorChangedCalled = true; };
            viewModel.Selections.Add("a");
            viewModel.Selections.Add("c");
            Assert.IsFalse(errorChangedCalled);
        }

        [TestMethod]
        public void WhenAddingNewSelectionExceedsMaxSelectionsThenErrorsChangedIsFired()
        {
            var question =
                new Question(new MultipleSelectionQuestionTemplate { MaxSelections = 2, Range = { "a", "b", "c" } });
            var viewModel = new MultipleSelectionQuestionViewModel(question);

            bool errorChangedCalled = false;
            ((INotifyDataErrorInfo)viewModel).ErrorsChanged += (s, e) => { errorChangedCalled = true; };
            viewModel.Selections.Add("a");
            viewModel.Selections.Add("c");
            viewModel.Selections.Add("b");

            // Assertions
            Assert.IsTrue(errorChangedCalled);
        }

        [TestMethod]
        public void WhenAddingSelectionThatAlreadyExceedsMaxSelectionsThenErrorsChangedIsNotFired()
        {
            var question =
                new Question(new MultipleSelectionQuestionTemplate { MaxSelections = 2, Range = { "a", "b", "c", "d" } });
            var viewModel = new MultipleSelectionQuestionViewModel(question);

            bool errorChangedCalled = false;
            ((INotifyDataErrorInfo)viewModel).ErrorsChanged += (s, e) => { errorChangedCalled = true; };
            viewModel.Selections.Add("a");
            viewModel.Selections.Add("c");
            viewModel.Selections.Add("b");

            errorChangedCalled = false;
            viewModel.Selections.Add("d");

            // Assertions
            Assert.IsFalse(errorChangedCalled);
        }

        [TestMethod]
        public void WhenRemovingSelectionFromAlreadyExceedingMaxSelectionsThenErrorsChangedIsFired()
        {
            var question =
                new Question(new MultipleSelectionQuestionTemplate { MaxSelections = 2, Range = { "a", "b", "c" } });
            var viewModel = new MultipleSelectionQuestionViewModel(question);

            viewModel.Selections.Add("a");
            viewModel.Selections.Add("c");
            viewModel.Selections.Add("b");
            bool errorChangedCalled = false;
            ((INotifyDataErrorInfo)viewModel).ErrorsChanged += (s, e) => { errorChangedCalled = true; };

            viewModel.Selections.Remove("a");

            // Assertions
            Assert.IsTrue(errorChangedCalled);
        }
    }
}
