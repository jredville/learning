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
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace StockTraderRI.Infrastructure.Tests
{
    [TestClass]
    public class OnValidationErrorFixture
    {
        [TestMethod]
        public void ShouldChangeBackgroundOnValidationError()
        {
            var element = new TextBox();
            var background = new SolidColorBrush();
            var model = new MockModel();
            OnValidationError.SetToggleBackground(element, background);
            CreateBindingThatValidatesOnExceptions(element, model);

            element.Text = "InvalidValue";

            Assert.AreEqual(background, element.Background);
        }

        [TestMethod]
        public void ShouldChangeToOriginalBackgroundErrorRemoved()
        {
            var element = new TextBox();
            var originalBackground = new SolidColorBrush();
            element.Background = originalBackground;
            var model = new MockModel();
            OnValidationError.SetToggleBackground(element, new SolidColorBrush());
            CreateBindingThatValidatesOnExceptions(element, model);
            element.Text = "InvalidValue";
            Assert.AreNotEqual(originalBackground, element.Background);

            element.Text = "ValidValue";

            Assert.AreEqual(originalBackground, element.Background);
        }

        [TestMethod]
        public void ShouldSetToolTipOnError()
        {
            var element = new TextBox();
            var model = new MockModel();
            model.ExceptionMessage = "My custom Exception message";
            OnValidationError.SetShowToolTip(element, true);
            CreateBindingThatValidatesOnExceptions(element, model);
            
            var originalTooltip = ToolTipService.GetToolTip(element);

            element.Text = "InvalidValue";

            Assert.IsNotNull(ToolTipService.GetToolTip(element));
            Assert.AreEqual(model.ExceptionMessage, ToolTipService.GetToolTip(element));
        }

        [TestMethod]
        public void ShouldRemoveToolTipOnErrorRemoved()
        {
            var element = new TextBox();
            var model = new MockModel();
            OnValidationError.SetShowToolTip(element, true);
            CreateBindingThatValidatesOnExceptions(element, model);

            var originalTooltip = ToolTipService.GetToolTip(element);

            element.Text = "InvalidValue";
            Assert.IsNotNull(ToolTipService.GetToolTip(element));

            element.Text = "ValidValue";

            Assert.AreEqual(originalTooltip, ToolTipService.GetToolTip(element));
        }

        [TestMethod]
        public void ShouldSetToolTipToOriginalOnErrorRemoved()
        {
            var element = new TextBox();
            string originalToolTip = "Please enter a valid value";
            ToolTipService.SetToolTip(element, originalToolTip);
            var model = new MockModel();
            OnValidationError.SetShowToolTip(element, true);
            CreateBindingThatValidatesOnExceptions(element, model);
            element.Text = "InvalidValue";
            Assert.IsNotNull(ToolTipService.GetToolTip(element));

            element.Text = "ValidValue";

            Assert.AreEqual(originalToolTip, ToolTipService.GetToolTip(element));
        }

        private static void CreateBindingThatValidatesOnExceptions(TextBox element, object source)
        {
            var binding = new Binding("Property");
            binding.Source = source;
            binding.Mode = BindingMode.TwoWay;
            binding.NotifyOnValidationError = true;
            binding.ValidatesOnExceptions = true;
            element.SetBinding(TextBox.TextProperty, binding);
        }

        public class MockModel 
        {
            public string ExceptionMessage = "Error Text";
            private string property;

            public string Property
            {
                get { return this.property; }
                set
                {
                    if (value == "InvalidValue")
                    {
                        throw new Exception(this.ExceptionMessage);
                    }

                    this.property = value;
                }
            }
        }
    }
}
