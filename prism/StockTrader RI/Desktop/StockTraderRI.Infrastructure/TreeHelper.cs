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
using System.Windows;

namespace StockTraderRI.Infrastructure
{
    /// <summary>
    /// Helper class used to traverse the Visual Tree.
    /// </summary>
    public static class TreeHelper
    {
        /// <summary>
        /// Traverses the Visual Tree upwards looking for the ancestor that satisfies the <paramref name="predicate"/>.
        /// </summary>
        /// <param name="dependencyObject">The element for which the ancestor is being looked for.</param>
        /// <param name="predicate">The predicate that evaluates if an element is the ancestor that is being looked for.</param>
        /// <returns>
        /// The ancestor element that matches the <paramref name="predicate"/> or <see langword="null"/>
        /// if the ancestor was not found.
        /// </returns>
        public static DependencyObject FindAncestor(DependencyObject dependencyObject, Func<DependencyObject, bool> predicate)
        {
            if (predicate(dependencyObject))
            {
                return dependencyObject;
            }

            DependencyObject parent = null;
#if SILVERLIGHT
            FrameworkElement frameworkElement = dependencyObject as FrameworkElement;
            if (frameworkElement != null)
            {
                parent = frameworkElement.Parent ?? System.Windows.Media.VisualTreeHelper.GetParent(frameworkElement);
            }
#else
            parent = LogicalTreeHelper.GetParent(dependencyObject);
#endif
            if (parent != null)
            {
                return FindAncestor(parent, predicate);
            }

            return null;
        }
    }
}