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
using System.Windows;

namespace StockTraderRI.Infrastructure.Tests
{
    public class DependencyPropertyHelper
    {
        public static T GetOrAddValue<T>(DependencyObject dependencyObject, DependencyProperty property) where T : class, new()
        {
            T value = dependencyObject.GetValue(property) as T;
            if (value == null)
            {
                value = new T();
                dependencyObject.SetValue(property, value);
            }

            return value;
        }
    }
}
