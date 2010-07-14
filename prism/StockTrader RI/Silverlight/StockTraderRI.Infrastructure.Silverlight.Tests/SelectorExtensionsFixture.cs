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
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Data;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace StockTraderRI.Infrastructure.Tests
{
    [TestClass]
    public class SelectorExtensionsFixture
    {
        [TestMethod]
        public void ShouldUpdateModelOnSelectionChange()
        {
            var model = new MockModel();
            var item1 = new MockItem { Property = 1 };
            var item2 = new MockItem { Property = 2 };
            var itemsSource = new List<MockItem> { item1, item2 };

            var selector = new ComboBox();
            selector.ItemsSource = itemsSource;
            SelectorExtensions.SetSelectedValuePath(selector, "Property");

            Binding valueBinding = new Binding("SelectedValueInModel");
            valueBinding.Mode = BindingMode.TwoWay;
            valueBinding.Source = model;
            selector.SetBinding(SelectorExtensions.SelectedValueProperty, valueBinding);

            selector.SelectedItem = item1;
            Assert.AreEqual(item1.Property, model.SelectedValueInModel);

            selector.SelectedItem = item2;
            Assert.AreEqual(item2.Property, model.SelectedValueInModel);
        }

        [TestMethod]
        public void ShouldInitiallySetSelectedItemInSelectorFromModel()
        {
            var model = new MockModel();
            var item1 = new MockItem { Property = 1 };
            var item2 = new MockItem { Property = 2 };
            var itemsSource = new List<MockItem> { item1, item2 };

            var selector = new ComboBox();
            selector.ItemsSource = itemsSource;
            SelectorExtensions.SetSelectedValuePath(selector, "Property");

            model.SelectedValueInModel = 2;

            Binding valueBinding = new Binding("SelectedValueInModel");
            valueBinding.Mode = BindingMode.TwoWay;
            valueBinding.Source = model;
            selector.SetBinding(SelectorExtensions.SelectedValueProperty, valueBinding);

            Assert.AreSame(item2, selector.SelectedItem);
        }

        [TestMethod]
        public void ShouldUpdateSelectedItemInSelectorWhenPropertyChanges()
        {
            var model = new MockModel();
            var item1 = new MockItem { Property = 1 };
            var item2 = new MockItem { Property = 2 };
            var itemsSource = new List<MockItem> { item1, item2 };

            var selector = new ComboBox();
            selector.ItemsSource = itemsSource;
            SelectorExtensions.SetSelectedValuePath(selector, "Property");

            Binding valueBinding = new Binding("SelectedValueInModel");
            valueBinding.Mode = BindingMode.TwoWay;
            valueBinding.Source = model;
            selector.SetBinding(SelectorExtensions.SelectedValueProperty, valueBinding);

            Assert.AreNotSame(item2, selector.SelectedItem);

            model.SelectedValueInModel = 2;

            Assert.AreSame(item2, selector.SelectedItem);
        }

        [TestMethod]
        public void ShouldNotFailIfItemsSourceIsNotSetBeforeSettingTheBindingToModel()
        {
            var model = new MockModel();
            var item1 = new MockItem { Property = 1 };
            var item2 = new MockItem { Property = 2 };
            var itemsSource = new List<MockItem> { item1, item2 };

            var selector = new ComboBox();
            SelectorExtensions.SetSelectedValuePath(selector, "Property");
            Binding valueBinding = new Binding("SelectedValueInModel");
            valueBinding.Mode = BindingMode.TwoWay;
            valueBinding.Source = model;
            selector.SetBinding(SelectorExtensions.SelectedValueProperty, valueBinding);

            selector.ItemsSource = itemsSource;
        }

        [TestMethod]
        public void ShouldSetSelectedItemWhenSettingValuePathAfterBindingToModel()
        {
            var model = new MockModel();
            var item1 = new MockItem { Property = 1 };
            var item2 = new MockItem { Property = 2 };
            var itemsSource = new List<MockItem> { item1, item2 };
            var selector = new ComboBox();
            selector.ItemsSource = itemsSource;
            model.SelectedValueInModel = 2;

            Binding valueBinding = new Binding("SelectedValueInModel");
            valueBinding.Mode = BindingMode.TwoWay;
            valueBinding.Source = model;
            selector.SetBinding(SelectorExtensions.SelectedValueProperty, valueBinding);
            Assert.AreNotSame(item2, selector.SelectedItem);

            SelectorExtensions.SetSelectedValuePath(selector, "Property");

            Assert.AreSame(item2, selector.SelectedItem);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void InvalidValuePathThrows()
        {
            var model = new MockModel();
            var item1 = new MockItem { Property = 1 };
            var itemsSource = new List<MockItem> { item1 };
            var selector = new ComboBox();
            selector.ItemsSource = itemsSource;

            Binding valueBinding = new Binding("SelectedValueInModel");
            valueBinding.Mode = BindingMode.TwoWay;
            valueBinding.Source = model;
            selector.SetBinding(SelectorExtensions.SelectedValueProperty, valueBinding);

            SelectorExtensions.SetSelectedValuePath(selector, "InvalidProperty");
        }

        public class MockItem
        {
            public int Property { get; set; }
        }

        public class MockModel : INotifyPropertyChanged
        {
            private int selectedValueInModel;

            public int SelectedValueInModel
            {
                get { return this.selectedValueInModel; }
                set
                {
                    if (this.selectedValueInModel != value)
                    {
                        this.selectedValueInModel = value;
                        if (this.PropertyChanged != null)
                            this.PropertyChanged(this, new PropertyChangedEventArgs("SelectedValueInModel"));
                    }
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;
        }
    }
}