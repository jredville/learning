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
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;

namespace StockTraderRI.Controls
{
    /// <summary>
    /// Custom ItemsControl with Header.
    /// </summary>
    public class ResearchControl : ItemsControl
    {
        public static readonly DependencyProperty HeadersProperty =
            DependencyProperty.Register("Headers", typeof(ObservableCollection<object>), typeof(ResearchControl), null);

        public ResearchControl()
        {
            this.Headers = new ObservableCollection<object>();
        }

        public ObservableCollection<object> Headers
        {
            get { return (ObservableCollection<object>)GetValue(HeadersProperty); }
            private set { SetValue(HeadersProperty, value); }
        }

        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                object newItem = e.NewItems[0];
                DependencyObject header = GetHeader(newItem as FrameworkElement);
                this.Headers.Insert(e.NewStartingIndex, header);
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                this.Headers.RemoveAt(e.OldStartingIndex);
            }
        }

        private static DependencyObject GetHeader(FrameworkElement view)
        {
            if (view != null)
            {
                DataTemplate template = view.Resources["HeaderIcon"] as DataTemplate;
                if (template != null)
                {
                    return template.LoadContent();
                }
            }

            return null;
        }
    }
}
