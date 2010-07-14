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
using System.Globalization;
using System.Windows.Data;

namespace StockTraderRI.Infrastructure.Converters
{
    public class EnumToBooleanConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var stringParameter = parameter as string;
            if (value == null || string.IsNullOrEmpty(stringParameter))
            {
                return false;
            }

            return string.Equals(value.ToString(), stringParameter, StringComparison.Ordinal);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var stringParameter = parameter as string;
            if (value == null || string.IsNullOrEmpty(stringParameter))
            {
                return null;
            }

            object parsedParameter = Enum.Parse(targetType, stringParameter, true);
            if (parsedParameter == null)
            {
                return null;
            }

            if ((bool)value)
            {
                return parsedParameter;
            }

            return null;
        }
        #endregion
    }
}
