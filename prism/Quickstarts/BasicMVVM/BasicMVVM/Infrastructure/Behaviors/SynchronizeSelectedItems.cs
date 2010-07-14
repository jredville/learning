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
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace BasicMVVM.Infrastructure.Behaviors
{
    /// <summary>
    /// Custom behavior to "bind" the check/unchecked state on a CheckBox to the presence or absence of an element
    /// in a list.
    /// </remarks>
    public class SynchronizeSelectedItems : Behavior<CheckBox>
    {
        public static readonly DependencyProperty SelectionsProperty = DependencyProperty.Register(
          "Selections",
          typeof(IList),
          typeof(SynchronizeSelectedItems),
          new PropertyMetadata(null));

        public IList Selections
        {
            get { return (IList)this.GetValue(SelectionsProperty); }
            set { this.SetValue(SelectionsProperty, value); }
        }

        protected override void OnAttached()
        {
            base.OnAttached();

            this.AssociatedObject.Checked += this.OnStatusChanged;
            this.AssociatedObject.Unchecked += this.OnStatusChanged;
            this.AssociatedObject.Indeterminate += this.OnStatusChanged;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            this.AssociatedObject.Checked -= this.OnStatusChanged;
            this.AssociatedObject.Unchecked -= this.OnStatusChanged;
            this.AssociatedObject.Indeterminate -= this.OnStatusChanged;
        }

        private void OnStatusChanged(object sender, RoutedEventArgs args)
        {
            if (this.Selections != null)
            {
                var element = this.AssociatedObject.Content;

                if (this.AssociatedObject.IsChecked.HasValue && this.AssociatedObject.IsChecked.Value)
                {
                    if (!this.Selections.Contains(element))
                    {
                        this.Selections.Add(element);
                    }
                }
                else
                {
                    if (this.Selections.Contains(element))
                    {
                        this.Selections.Remove(element);
                    }
                }
            }
        }
    }
}
