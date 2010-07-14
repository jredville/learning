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
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BasicMVVM.Test.ViewModels
{
    public class NotifyDataErrorInfoTestHelper<T, TProperty>
        where T : INotifyDataErrorInfo
    {
        private readonly T viewModel;
        private readonly string propertyName;
        private bool errorsChangedNotified;

        public NotifyDataErrorInfoTestHelper(
            T viewModel,
            Expression<Func<T, TProperty>> propertyExpression)
        {
            this.viewModel = viewModel;
            this.propertyName = ExtractPropertyName(propertyExpression);
        }

        internal void ValidatePropertyChange(TProperty newValue, NotifyDataErrorInfoBehavior behavior)
        {
            this.viewModel.ErrorsChanged += HandleViewModelErrorsChanged;

            SetPropertyValueOnViewModel(newValue);

            var messageError = string.Format("When setting value to {0} ", newValue);

            Assert.AreEqual(IsSet(NotifyDataErrorInfoBehavior.FiresErrorsChanged, behavior), this.errorsChangedNotified, messageError + "fired ErrorsChanged event");
            Assert.AreEqual(IsSet(NotifyDataErrorInfoBehavior.HasErrors, behavior), this.viewModel.HasErrors, messageError + "HasErrors");
            Assert.AreEqual(
                IsSet(NotifyDataErrorInfoBehavior.HasErrorsForProperty, behavior),
                (this.viewModel.GetErrors(this.propertyName) ?? new object[0]).GetEnumerator().MoveNext(),
                messageError + "HasErrorsForProperty");

            this.viewModel.ErrorsChanged -= HandleViewModelErrorsChanged;
            this.errorsChangedNotified = false;
        }

        internal void ValidateAllPropertyChanges(TProperty validValue, TProperty anotherValidValue, TProperty invalidValue, TProperty anotherInvalidValue)
        {
            // Valid -> Valid
            this.SetPropertyValueOnViewModel(validValue);
            this.ValidatePropertyChange(anotherValidValue, NotifyDataErrorInfoBehavior.Nothing);

            // Valid -> Invalid
            this.SetPropertyValueOnViewModel(validValue);
            this.ValidatePropertyChange(invalidValue, NotifyDataErrorInfoBehavior.FiresErrorsChanged | NotifyDataErrorInfoBehavior.HasErrors | NotifyDataErrorInfoBehavior.HasErrorsForProperty);

            // Invalid -> Valid
            this.SetPropertyValueOnViewModel(invalidValue);
            this.ValidatePropertyChange(validValue, NotifyDataErrorInfoBehavior.FiresErrorsChanged);

            // Invalid -> Invalid
            this.SetPropertyValueOnViewModel(invalidValue);
            this.ValidatePropertyChange(anotherInvalidValue, NotifyDataErrorInfoBehavior.HasErrors | NotifyDataErrorInfoBehavior.HasErrorsForProperty);

        }

        private void HandleViewModelErrorsChanged(object sender, DataErrorsChangedEventArgs e)
        {
            if (e.PropertyName == this.propertyName)
            {
                this.errorsChangedNotified = true;
            }
        }

        private void SetPropertyValueOnViewModel(TProperty newPropertyValue)
        {
            this.viewModel.GetType()
                .InvokeMember(
                    this.propertyName,
                    BindingFlags.SetProperty | BindingFlags.Public | BindingFlags.Instance,
                    null,
                    this.viewModel,
                    new object[] { newPropertyValue });
        }

        private static bool IsSet(NotifyDataErrorInfoBehavior behavior, NotifyDataErrorInfoBehavior behaviors)
        {
            return (behaviors & behavior) == behavior;
        }

        private static string ExtractPropertyName(Expression<Func<T, TProperty>> propertyExpression)
        {
            return ((MemberExpression)propertyExpression.Body).Member.Name;
        }
    }

    [Flags]
    public enum NotifyDataErrorInfoBehavior
    {
        FiresErrorsChanged = 1,
        HasErrors = 2,
        HasErrorsForProperty = 4,
        Nothing = 8
    }
}
