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
using System.Windows.Media;
using System.Windows.Data;
using System.Windows.Controls;
using System.Windows;

namespace StockTraderRI.ChartControls
{
    public class ColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Random r;
            int index = 0;

            UIElement c = value as UIElement;
            Panel p = (Panel)VisualTreeHelper.GetParent(c);
            if(p == null)
            {
                r = new Random((int)DateTime.Now.Ticks);
            }
            else
            {
                ItemsControl _parent = ((ItemsControl)((FrameworkElement)VisualTreeHelper.GetParent(p)).TemplatedParent);
                index = _parent.ItemContainerGenerator.IndexFromContainer(c);
                r = new Random((int)index);
            }
            Color color = Color.FromScRgb(1.0f, (float)r.NextDouble(), (float)r.NextDouble(), (float)r.NextDouble());
            return new SolidColorBrush(color);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new Exception("The method or operation is not implemented.");
        }
    }
}
