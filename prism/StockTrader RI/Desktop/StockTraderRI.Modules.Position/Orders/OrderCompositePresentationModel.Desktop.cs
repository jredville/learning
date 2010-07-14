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
using System.Windows.Data;
using StockTraderRI.Infrastructure;
using StockTraderRI.Modules.Position.Models;

namespace StockTraderRI.Modules.Position.Orders
{
	public partial class OrderCompositePresentationModel
	{
        partial void SetTransactionInfo(TransactionInfo transactionInfo)
        {
            //This instance of TransactionInfo acts as a "shared model" between this view and the order details view.
            //The scenario says that these 2 views are decoupled, so they don't share the presentation model, they are only tied
            //with this TransactionInfo
            this.orderDetailsPresentationModel.TransactionInfo = transactionInfo;

            //Bind the CompositeOrderView header to a string representation of the TransactionInfo shared instance (we expect the details presenter to modify it from user interaction).
            MultiBinding binding = new MultiBinding();
            binding.Bindings.Add(new Binding("TransactionType") { Source = transactionInfo });
            binding.Bindings.Add(new Binding("TickerSymbol") { Source = transactionInfo });
            binding.Converter = new OrderHeaderConverter();
            BindingOperations.SetBinding(this, HeaderInfoProperty, binding);
        }

        public string HeaderInfo
        {
            get { return (string)GetValue(HeaderInfoProperty); }
            set { SetValue(HeaderInfoProperty, value); }
        }

        private class OrderHeaderConverter : IMultiValueConverter
        {
            /// <summary>
            /// Converts a <see cref="TransactionType"/> and a ticker symbol to a header that can be placed on the TabItem header
            /// </summary>
            /// <param name="values">values[0] should be of type <see cref="TransactionType"/>. values[1] should be a string with the ticker symbol</param>
            /// <param name="targetType"></param>
            /// <param name="parameter"></param>
            /// <param name="culture"></param>
            /// <returns>Returns a human readable string with the transaction type and ticker symbol</returns>
            public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            {
                return values[0].ToString() + " " + values[1].ToString();
            }

            public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }
	}
}
