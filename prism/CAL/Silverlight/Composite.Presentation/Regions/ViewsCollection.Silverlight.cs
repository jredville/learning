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
using System.Collections.Specialized;

namespace Microsoft.Practices.Composite.Presentation.Regions
{
    public partial class ViewsCollection
    {
        private void AddAndNotify(IList items)
        {
            if (items.Count > 0)
            {
                foreach (object item in items)
                {
                    int index = filteredCollection.Count;
                    filteredCollection.Add(item);
                    OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
                }
            }
        }

        private void RemoveAndNotify(IList items)
        {
            if (items.Count > 0)
            {
                foreach (object item in items)
                {
                    int index = filteredCollection.IndexOf(items[0]);
                    filteredCollection.Remove(item);
                    OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index));
                }
            }
        }
    }
}
