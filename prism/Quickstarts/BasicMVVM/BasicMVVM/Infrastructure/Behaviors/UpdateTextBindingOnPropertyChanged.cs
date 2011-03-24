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
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Interactivity;

namespace BasicMVVM.Infrastructure.Behaviors
{
    /// <summary>
    /// Custom behavior that updates the source of a binding on a text box as the text changes.
    /// </summary>
    public class UpdateTextBindingOnPropertyChanged : Behavior<TextBox>
    {
        private BindingExpression expression;

        protected override void OnAttached()
        {
            base.OnAttached();

            this.expression = this.AssociatedObject.GetBindingExpression(TextBox.TextProperty);
            this.AssociatedObject.TextChanged += this.OnTextChanged;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            this.AssociatedObject.TextChanged -= this.OnTextChanged;
            this.expression = null;
        }

        private void OnTextChanged(object sender, EventArgs args)
        {
            this.expression.UpdateSource();
        }
    }
}
