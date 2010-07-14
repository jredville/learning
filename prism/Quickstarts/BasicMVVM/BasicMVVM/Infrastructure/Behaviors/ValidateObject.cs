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
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace BasicMVVM.Infrastructure.Behaviors
{
    /// <summary>
    /// Custom behavior that links a <see cref="ValidationSummary"/> control to an object that implements the
    /// <see cref="INotifyDataErroInfo"/> interface to display top-level errors (i.e. those not related
    /// to properties).
    /// </summary>
    public class ValidateObject : Behavior<ValidationSummary>
    {
        public static readonly DependencyProperty SourceObjectProperty = DependencyProperty.Register(
          "SourceObject",
          typeof(object),
          typeof(ValidateObject),
          new PropertyMetadata(null, OnSourceObjectChanged));

        public ValidateObject()
        {
        }

        public object SourceObject
        {
            get { return (object)this.GetValue(SourceObjectProperty); }
            set { this.SetValue(SourceObjectProperty, value); }
        }

        protected override void OnAttached()
        {
            base.OnAttached();

            this.AttachToSourceObject(null, this.SourceObject);
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            this.AttachToSourceObject(this.SourceObject, null);
        }

        private static void OnSourceObjectChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var behavior = d as ValidateObject;
            if (behavior == null)
            {
                throw new ArgumentException("Not a ValidateObject instance", "d");
            }

            if (behavior.AssociatedObject == null)
            {
                return;
            }

            behavior.AttachToSourceObject((INotifyDataErrorInfo)e.OldValue, (INotifyDataErrorInfo)e.NewValue);
        }

        private void AttachToSourceObject(object oldValue, object newValue)
        {
            var oldIndei = oldValue as INotifyDataErrorInfo;
            if (oldIndei != null)
            {
                oldIndei.ErrorsChanged -= this.OnErrorsChanged;
            }

            var newIndei = newValue as INotifyDataErrorInfo;
            if (newIndei != null)
            {
                newIndei.ErrorsChanged += this.OnErrorsChanged;
            }
        }

        private void OnErrorsChanged(object sender, DataErrorsChangedEventArgs args)
        {
            var objectErrors = new List<ValidationSummaryItem>(this.AssociatedObject.Errors);
            foreach (var objectError in objectErrors.Where(e => e.ItemType == ValidationSummaryItemType.ObjectError))
            {
                this.AssociatedObject.Errors.Remove(objectError);
            }

            foreach (var newObjectError in ((INotifyDataErrorInfo)this.SourceObject).GetErrors(null) ?? new object[0])
            {
                this.AssociatedObject.Errors.Add(
                    new ValidationSummaryItem(
                        newObjectError.ToString(),
                        null,
                        ValidationSummaryItemType.ObjectError,
                        null,
                        null));
            }
        }
    }
}
