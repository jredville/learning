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

namespace BasicMVVM.Test.ViewModels
{
    [TestClass]
    public class NumericQuestionViewModelFixture
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void WhenCreatingAViewModelWithANullQuestion_ThenAnExceptionIsThrown()
        {
            new NumericQuestionViewModel(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void WhenCreatingAViewModelWithAnIncompatibleTemplate_ThenAnExceptionIsThrown()
        {
            new NumericQuestionViewModel(new Question(new MockQuestionTemplate()));
        }

        [TestMethod]
        public void WhenGettingTheResponseValueFromANewViewModel_ThenGetsNull()
        {
            var viewModel = new NumericQuestionViewModel(new Question(new NumericQuestionTemplate { MaxValue = 100 }));

            var responseValue = viewModel.ResponseValue;

            Assert.AreEqual(null, responseValue);
        }

        [TestMethod]
        public void WhenSettingTheResponseValueOnTheViewModel_ThenTheQuestionIsUpdated()
        {
            var question = new Question(new NumericQuestionTemplate { MaxValue = 100 });
            var viewModel = new NumericQuestionViewModel(question);

            viewModel.ResponseValue = "15";

            Assert.AreEqual(15, question.Response);
        }

        [TestMethod]
        public void WhenSettingTheResponseValueOnTheViewModel_ThenCanGetTheValueFromTheViewModel()
        {
            var question = new Question(new NumericQuestionTemplate { MaxValue = 100 });
            var viewModel = new NumericQuestionViewModel(question);

            viewModel.ResponseValue = "15";

            Assert.AreEqual("15", viewModel.ResponseValue);
        }


        [TestMethod]
        public void WhenANonFormattableValueIsSetAsTheResponseOnANewInstanceWithNoMaxValue_TheINotifyDataErrorInfoIsProperlyImplemented()
        {
            var question = new Question(new NumericQuestionTemplate());
            var viewModel = new NumericQuestionViewModel(question);

            var helper = new NotifyDataErrorInfoTestHelper<NumericQuestionViewModel, string>(viewModel, vm => vm.ResponseValue);

            helper.ValidatePropertyChange(
                "abc",
                NotifyDataErrorInfoBehavior.FiresErrorsChanged | NotifyDataErrorInfoBehavior.HasErrors | NotifyDataErrorInfoBehavior.HasErrorsForProperty);
        }

        [TestMethod]
        public void WhenAFormattableValueIsSetAsTheResponseOnANewInstanceWithNoMaxValue_TheINotifyDataErrorInfoIsProperlyImplemented()
        {
            var question = new Question(new NumericQuestionTemplate());
            var viewModel = new NumericQuestionViewModel(question);

            var helper = new NotifyDataErrorInfoTestHelper<NumericQuestionViewModel, string>(viewModel, vm => vm.ResponseValue);

            helper.ValidatePropertyChange(
                "111",
                NotifyDataErrorInfoBehavior.Nothing);
        }

        [TestMethod]
        public void WhenANonFormattableValueIsSetAsTheResponseOnAnInstanceWithANonFormatableValueAndWithNoMaxValue_TheINotifyDataErrorInfoIsProperlyImplemented()
        {
            var question = new Question(new NumericQuestionTemplate());
            var viewModel = new NumericQuestionViewModel(question) { ResponseValue = "abc" };

            var helper = new NotifyDataErrorInfoTestHelper<NumericQuestionViewModel, string>(viewModel, vm => vm.ResponseValue);

            helper.ValidatePropertyChange(
                "bca",
                NotifyDataErrorInfoBehavior.HasErrors | NotifyDataErrorInfoBehavior.HasErrorsForProperty);
        }

        [TestMethod]
        public void WhenAFormattableValueIsSetAsTheResponseOnAnInstanceWithANonFormatableValueAndWithNoMaxValue_TheINotifyDataErrorInfoIsProperlyImplemented()
        {
            var question = new Question(new NumericQuestionTemplate());
            var viewModel = new NumericQuestionViewModel(question) { ResponseValue = "abc" };

            var helper = new NotifyDataErrorInfoTestHelper<NumericQuestionViewModel, string>(viewModel, vm => vm.ResponseValue);

            helper.ValidatePropertyChange(
                "123",
                NotifyDataErrorInfoBehavior.FiresErrorsChanged);
        }

        [TestMethod]
        public void WhenANonFormattableValueIsSetAsTheResponseOnAnInstanceWithFormatableValueAndWithNoMaxValue_TheINotifyDataErrorInfoIsProperlyImplemented()
        {
            var question = new Question(new NumericQuestionTemplate());
            var viewModel = new NumericQuestionViewModel(question) { ResponseValue = "123" };

            var helper = new NotifyDataErrorInfoTestHelper<NumericQuestionViewModel, string>(viewModel, vm => vm.ResponseValue);

            helper.ValidatePropertyChange(
                "abc",
                NotifyDataErrorInfoBehavior.FiresErrorsChanged | NotifyDataErrorInfoBehavior.HasErrors | NotifyDataErrorInfoBehavior.HasErrorsForProperty);
        }

        [TestMethod]
        public void WhenAFormattableValueIsSetAsTheResponseOnAnInstanceWithAFormatableValueAndWithNoMaxValue_TheINotifyDataErrorInfoIsProperlyImplemented()
        {
            var question = new Question(new NumericQuestionTemplate());
            var viewModel = new NumericQuestionViewModel(question) { ResponseValue = "123" };

            var helper = new NotifyDataErrorInfoTestHelper<NumericQuestionViewModel, string>(viewModel, vm => vm.ResponseValue);

            helper.ValidatePropertyChange(
                "321",
                NotifyDataErrorInfoBehavior.Nothing);
        }


        [TestMethod]
        public void WhenANonFormattableValueIsSetAsTheResponseOnANewInstanceWithMaxValue_TheINotifyDataErrorInfoIsProperlyImplemented()
        {
            var question = new Question(new NumericQuestionTemplate() { MaxValue = 200 });
            var viewModel = new NumericQuestionViewModel(question);

            var helper = new NotifyDataErrorInfoTestHelper<NumericQuestionViewModel, string>(viewModel, vm => vm.ResponseValue);

            helper.ValidatePropertyChange(
                "abc",
                NotifyDataErrorInfoBehavior.FiresErrorsChanged | NotifyDataErrorInfoBehavior.HasErrors | NotifyDataErrorInfoBehavior.HasErrorsForProperty);
        }

        [TestMethod]
        public void WhenAFormattableValueIsSetAsTheResponseOnANewInstanceWithMaxValue_TheINotifyDataErrorInfoIsProperlyImplemented()
        {
            var question = new Question(new NumericQuestionTemplate() { MaxValue = 200 });
            var viewModel = new NumericQuestionViewModel(question);

            var helper = new NotifyDataErrorInfoTestHelper<NumericQuestionViewModel, string>(viewModel, vm => vm.ResponseValue);

            helper.ValidatePropertyChange(
                "111",
                NotifyDataErrorInfoBehavior.Nothing);
        }

        [TestMethod]
        public void WhenAFormattableOverTheMaxValueIsSetAsTheResponseOnANewInstanceWithMaxValue_TheINotifyDataErrorInfoIsProperlyImplemented()
        {
            var question = new Question(new NumericQuestionTemplate() { MaxValue = 200 });
            var viewModel = new NumericQuestionViewModel(question);

            var helper = new NotifyDataErrorInfoTestHelper<NumericQuestionViewModel, string>(viewModel, vm => vm.ResponseValue);

            helper.ValidatePropertyChange(
                "500",
                NotifyDataErrorInfoBehavior.FiresErrorsChanged | NotifyDataErrorInfoBehavior.HasErrorsForProperty | NotifyDataErrorInfoBehavior.HasErrors);
        }

        [TestMethod]
        public void WhenANonFormattableValueIsSetAsTheResponseOnAnInstanceWithANonFormatableValueAndWithMaxValue_TheINotifyDataErrorInfoIsProperlyImplemented()
        {
            var question = new Question(new NumericQuestionTemplate() { MaxValue = 200 });
            var viewModel = new NumericQuestionViewModel(question) { ResponseValue = "bca" };

            var helper = new NotifyDataErrorInfoTestHelper<NumericQuestionViewModel, string>(viewModel, vm => vm.ResponseValue);

            helper.ValidatePropertyChange(
                "abc",
                NotifyDataErrorInfoBehavior.HasErrors | NotifyDataErrorInfoBehavior.HasErrorsForProperty);
        }

        [TestMethod]
        public void WhenAFormattableValueIsSetAsTheResponseOnAnInstanceWithANonFormatableValueAndWithWithMaxValue_TheINotifyDataErrorInfoIsProperlyImplemented()
        {
            var question = new Question(new NumericQuestionTemplate() { MaxValue = 200 });
            var viewModel = new NumericQuestionViewModel(question) { ResponseValue = "bca" };

            var helper = new NotifyDataErrorInfoTestHelper<NumericQuestionViewModel, string>(viewModel, vm => vm.ResponseValue);

            helper.ValidatePropertyChange(
                "111",
                NotifyDataErrorInfoBehavior.FiresErrorsChanged);
        }

        [TestMethod]
        public void WhenAFormattableOverTheMaxValueIsSetAsTheResponseOnAnInstanceWithANonFormatableValueAndWithWithMaxValue_TheINotifyDataErrorInfoIsProperlyImplemented()
        {
            var question = new Question(new NumericQuestionTemplate() { MaxValue = 200 });
            var viewModel = new NumericQuestionViewModel(question) { ResponseValue = "bca" };

            var helper = new NotifyDataErrorInfoTestHelper<NumericQuestionViewModel, string>(viewModel, vm => vm.ResponseValue);

            helper.ValidatePropertyChange(
                "500",
                NotifyDataErrorInfoBehavior.FiresErrorsChanged | NotifyDataErrorInfoBehavior.HasErrorsForProperty | NotifyDataErrorInfoBehavior.HasErrors);
        }

        [TestMethod]
        public void WhenANonFormattableValueIsSetAsTheResponseOnAnInstanceWithAFormatableValueAndWithMaxValue_TheINotifyDataErrorInfoIsProperlyImplemented()
        {
            var question = new Question(new NumericQuestionTemplate() { MaxValue = 200 });
            var viewModel = new NumericQuestionViewModel(question) { ResponseValue = "11" };

            var helper = new NotifyDataErrorInfoTestHelper<NumericQuestionViewModel, string>(viewModel, vm => vm.ResponseValue);

            helper.ValidatePropertyChange(
                "abc",
                NotifyDataErrorInfoBehavior.FiresErrorsChanged | NotifyDataErrorInfoBehavior.HasErrors | NotifyDataErrorInfoBehavior.HasErrorsForProperty);
        }

        [TestMethod]
        public void WhenAFormattableValueIsSetAsTheResponseOnAnInstanceWithAFormatableValueAndWithWithMaxValue_TheINotifyDataErrorInfoIsProperlyImplemented()
        {
            var question = new Question(new NumericQuestionTemplate() { MaxValue = 200 });
            var viewModel = new NumericQuestionViewModel(question) { ResponseValue = "11" };

            var helper = new NotifyDataErrorInfoTestHelper<NumericQuestionViewModel, string>(viewModel, vm => vm.ResponseValue);

            helper.ValidatePropertyChange(
                "111",
                NotifyDataErrorInfoBehavior.Nothing);
        }

        [TestMethod]
        public void WhenAFormattableOverTheMaxValueIsSetAsTheResponseOnAnInstanceWithAFormatableValueAndWithWithMaxValue_TheINotifyDataErrorInfoIsProperlyImplemented()
        {
            var question = new Question(new NumericQuestionTemplate() { MaxValue = 200 });
            var viewModel = new NumericQuestionViewModel(question) { ResponseValue = "11" };

            var helper = new NotifyDataErrorInfoTestHelper<NumericQuestionViewModel, string>(viewModel, vm => vm.ResponseValue);

            helper.ValidatePropertyChange(
                "500",
                NotifyDataErrorInfoBehavior.FiresErrorsChanged | NotifyDataErrorInfoBehavior.HasErrorsForProperty | NotifyDataErrorInfoBehavior.HasErrors);
        }

        [TestMethod]
        public void WhenANonFormattableValueIsSetAsTheResponseOnAnInstanceWithAFormatableValueOverTheMaxValueAndWithMaxValue_TheINotifyDataErrorInfoIsProperlyImplemented()
        {
            var question = new Question(new NumericQuestionTemplate() { MaxValue = 200 });
            var viewModel = new NumericQuestionViewModel(question) { ResponseValue = "300" };

            var helper = new NotifyDataErrorInfoTestHelper<NumericQuestionViewModel, string>(viewModel, vm => vm.ResponseValue);

            helper.ValidatePropertyChange(
                "abc",
                NotifyDataErrorInfoBehavior.FiresErrorsChanged | NotifyDataErrorInfoBehavior.HasErrors | NotifyDataErrorInfoBehavior.HasErrorsForProperty);
        }

        [TestMethod]
        public void WhenAFormattableValueIsSetAsTheResponseOnAnInstanceWithAFormatableValueOverTheMaxValueAndWithWithMaxValue_TheINotifyDataErrorInfoIsProperlyImplemented()
        {
            var question = new Question(new NumericQuestionTemplate() { MaxValue = 200 });
            var viewModel = new NumericQuestionViewModel(question) { ResponseValue = "300" };

            var helper = new NotifyDataErrorInfoTestHelper<NumericQuestionViewModel, string>(viewModel, vm => vm.ResponseValue);

            helper.ValidatePropertyChange(
                "111",
                NotifyDataErrorInfoBehavior.FiresErrorsChanged);
        }

        [TestMethod]
        public void WhenAFormattableOverTheMaxValueIsSetAsTheResponseOnAnInstanceWithAFormatableValueOverTheMaxValueAndWithWithMaxValue_TheINotifyDataErrorInfoIsProperlyImplemented()
        {
            var question = new Question(new NumericQuestionTemplate() { MaxValue = 200 });
            var viewModel = new NumericQuestionViewModel(question) { ResponseValue = "300" };

            var helper = new NotifyDataErrorInfoTestHelper<NumericQuestionViewModel, string>(viewModel, vm => vm.ResponseValue);

            helper.ValidatePropertyChange(
                "500",
                NotifyDataErrorInfoBehavior.HasErrorsForProperty | NotifyDataErrorInfoBehavior.HasErrors);
        }
    }
}
