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
    public class DecimalToColorConverterFixture
    {
        [TestMethod]
        public void ShouldConvertFromDecimalToColorString()
        {
            DecimalToColorConverter converter = new DecimalToColorConverter();

            var convertedValue = converter.Convert(20m, null, null, null) as string;
            Assert.IsNotNull(convertedValue);
            Assert.AreEqual("#ff00cc00", convertedValue);

            convertedValue = converter.Convert(-20m, null, null, null) as string;
            Assert.IsNotNull(convertedValue);
            Assert.AreEqual("#ffff0000", convertedValue);
        }

        [TestMethod]
        public void ShouldReturnNullIfValueToConvertIsNullOrNotDecimal()
        {
            DecimalToColorConverter converter = new DecimalToColorConverter();

            var convertedValue = converter.Convert(null, null, null, null) as string;
            Assert.IsNull(convertedValue);

            convertedValue = converter.Convert("NotADecimal", null, null, null) as string;
            Assert.IsNull(convertedValue);
        }
    }
}
