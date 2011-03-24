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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StockTraderRI.Infrastructure.Converters;

namespace StockTraderRI.Infrastructure.Tests.Converters
{
    [TestClass]
    public class EnumToBooleanConverterFixture
    {
        [TestMethod]
        public void EnumToBooleanConverterConverts()
        {
            EnumToBooleanConverter converter = new EnumToBooleanConverter();
            object value = converter.Convert(TransactionType.Buy, typeof(TransactionType), "Buy", null);

            Assert.IsTrue((bool)value);
        }

        [TestMethod]
        public void EnumToBooleanConverterConvertsBack()
        {
            EnumToBooleanConverter converter = new EnumToBooleanConverter();
            object value = converter.ConvertBack(true, typeof(TransactionType), "Sell", null);

            Assert.AreEqual(TransactionType.Sell, value);
        }
    }
}