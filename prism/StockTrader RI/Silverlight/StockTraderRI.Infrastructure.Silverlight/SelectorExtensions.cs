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
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using StockTraderRI.Infrastructure.Properties;

namespace StockTraderRI.Infrastructure
{
    /// <summary>
    /// Defines the Attached Behavior needed to keep synchronized the selected value 
    /// of a <see cref="Selector"/> with a bound property.
    /// </summary>
    /// <remarks>This is to workaround the missing SelectedItem property that is present in WPF but not in Silverlight.</remarks>
    public static class SelectorExtensions
    {
        /// <summary>
        /// The property bound to the <see cref="Selector"/>'s selected value.
        /// </summary>
        public static readonly DependencyProperty SelectedValueProperty =
                DependencyProperty.RegisterAttached("SelectedValue", typeof(object), typeof(SelectorExtensions), new PropertyMetadata(SelectedValueChanged));

        /// <summary>
        /// The path to the bound property getter.
        /// </summary>
        public static readonly DependencyProperty SelectedValuePathProperty =
                DependencyProperty.RegisterAttached("SelectedValuePath", typeof(string), typeof(SelectorExtensions), new PropertyMetadata(SelectedValuePathChanged));

        private static readonly Dictionary<string, MethodInfo> cachedPropertyGetters = new Dictionary<string, MethodInfo>();

        /// <summary>
        /// Gets the <see cref="SelectedValueProperty"/> value.
        /// </summary>
        /// <param name="dependencyObject">The <see cref="DependencyObject"/> on which the attached property is set.</param>
        /// <returns>The value of the <see cref="SelectedValueProperty"/> property.</returns>
        public static object GetSelectedValue(DependencyObject dependencyObject)
        {
            return dependencyObject.GetValue(SelectedValueProperty);
        }

        /// <summary>
        /// Sets the <see cref="SelectedValueProperty"/> value.
        /// </summary>
        /// <param name="dependencyObject">The <see cref="DependencyObject"/> on which the attached property will be set.</param>
        /// <param name="value">Value to set to <see cref="SelectedValueProperty"/> attached property.</param>
        public static void SetSelectedValue(DependencyObject dependencyObject, object value)
        {
            dependencyObject.SetValue(SelectedValueProperty, value);
        }

        /// <summary>
        /// Gets the <see cref="SelectedValuePathProperty"/> value.
        /// </summary>
        /// <param name="dependencyObject">The <see cref="DependencyObject"/> on which the attached property is set.</param>
        /// <returns>The value of the <see cref="SelectedValuePathProperty"/> property.</returns>
        public static string GetSelectedValuePath(DependencyObject dependencyObject)
        {
            return dependencyObject.GetValue(SelectedValuePathProperty) as string;
        }

        /// <summary>
        /// Sets the <see cref="SelectedValuePathProperty"/> value.
        /// </summary>
        /// <param name="dependencyObject">The <see cref="DependencyObject"/> on which the attached property will be set.</param>
        /// <param name="value">Value to set to <see cref="SelectedValuePathProperty"/> attached property.</param>
        public static void SetSelectedValuePath(DependencyObject dependencyObject, string value)
        {
            dependencyObject.SetValue(SelectedValuePathProperty, value);
        }

        private static void SelectedValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            object newValue = e.NewValue;
            if (!Equals(newValue, e.OldValue))
            {
                Selector selector = d as Selector;
                if (selector != null)
                {
                    SyncronizeSelectedItem(selector, newValue);
                }
            }
        }

        private static void SyncronizeSelectedItem(Selector selector, object value)
        {
            string selectedValuePath = GetSelectedValuePath(selector);
            if (!string.IsNullOrEmpty(selectedValuePath))
            {
                if (selector.SelectedItem == null
                    || !Equals(GetValueForPath(selector.SelectedItem, selectedValuePath), value))
                {
                    object selectedItem =
                            selector.Items.FirstOrDefault(
                                    item => Equals(GetValueForPath(item, selectedValuePath), value));
                    selector.SelectedItem = selectedItem;
                }
            }
        }

        private static void SelectedValuePathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Selector selector = d as Selector;
            if (selector != null)
            {
                if (e.OldValue == null && e.NewValue != null)
                {
                    // Subscribes to selection changes in the Selector.
                    selector.SelectionChanged += SelectorSelectionChanged;
                }

                if (e.OldValue != null && e.NewValue == null)
                {
                    // Unsubscribes to clean up.
                    selector.SelectionChanged -= SelectorSelectionChanged;
                }

                SyncronizeSelectedItem(selector, GetSelectedValue(selector));
            }
        }

        private static void SelectorSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Selector selector = (Selector)sender;
            object selectedItem = selector.SelectedItem;
            string selectedValuePath = GetSelectedValuePath(selector);
            object selectedValue = GetValueForPath(selectedItem, selectedValuePath);
            if (!Equals(GetSelectedValue(selector), selectedValue))
            {
                SetSelectedValue(selector, selectedValue);
            }
        }

        private static object GetValueForPath(object instance, string valuePath)
        {
            MethodInfo propertyGetter = GetPropertyGetterForType(instance.GetType(), valuePath);
            object returnValue = propertyGetter.Invoke(instance, null);
            return returnValue;
        }

        private static MethodInfo GetPropertyGetterForType(Type type, string memberName)
        {
            string hashKey = string.Format(CultureInfo.InvariantCulture, "{0}&{1}", type.AssemblyQualifiedName, memberName);
            MethodInfo methodInfo;
            if (!cachedPropertyGetters.TryGetValue(hashKey, out methodInfo))
            {
                PropertyInfo property = type.GetProperty(memberName);
                if (property != null)
                {
                    methodInfo = property.GetGetMethod();
                }

                cachedPropertyGetters.Add(hashKey, methodInfo);
            }

            if (methodInfo == null)
            {
                throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture, Resources.SelectorExtensionCannotResolveMember, type.FullName, memberName, typeof(SelectorExtensions).Name));
            }

            return methodInfo;
        }
    }
}