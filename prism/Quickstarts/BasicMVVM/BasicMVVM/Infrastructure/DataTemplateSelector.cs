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
using System.Windows.Controls;

namespace BasicMVVM.Infrastructure
{
    /// <summary>
    /// This custom ContentControl changes its ContentTemplate based on the content it is presenting.
    /// </summary>
    /// <remarks>
    /// In order to determine the template it must use for the new content, this control retrieves it from its
    /// resources using the the name for the type of the new content as the key.
    /// </remarks>
    public class DataTemplateSelector : ContentControl
    {
        protected override void OnContentChanged(object oldContent, object newContent)
        {
            DataTemplate contentTemplate = null;
            if (newContent != null)
            {
                var contentTypeName = newContent.GetType().Name;
                contentTemplate = this.Resources[contentTypeName] as DataTemplate;
            }

            this.ContentTemplate = contentTemplate;
        }
    }
}
