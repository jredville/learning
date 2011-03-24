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
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace StockTraderRI.Infrastructure
{
    /// <summary>
    /// Defines a <see cref="DependencyProperty"/> where all <see cref="ValidationError"/> are stored to be used from XAML on <see cref="ToolTip"/>.
    /// </summary>
    public static class Validation
    {
        /// <summary>
        /// Collection of <see cref="ValidationError"/> ocurred on the attached element.
        /// </summary>
        public static readonly DependencyProperty ErrorsProperty = DependencyProperty.RegisterAttached(
                "Errors", typeof(ObservableCollection<ValidationError>), typeof(Validation), null);

        /// <summary>
        /// Gets the value of <see cref="ErrorsProperty"/> on <paramref name="dependencyObject"/>.
        /// </summary>
        /// <param name="dependencyObject">Element on which the <see cref="ErrorsProperty"/> property is attached.</param>
        /// <returns>Value of <see cref="ErrorsProperty"/>.</returns>
        public static ObservableCollection<ValidationError> GetErrors(DependencyObject dependencyObject)
        {
            return DependencyPropertyHelper.GetOrAddValue<ObservableCollection<ValidationError>>(dependencyObject, ErrorsProperty);
        }
    }
}