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
using BasicMVVM.Infrastructure.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq.Expressions;

namespace BasicMVVM.Test.Infrastructure
{
    [TestClass]
    public class ViewModelFixture : ViewModel
    {
        [TestMethod]
        public void WhenNotifyingOnAnInstanceProperty_ThenAnEventIsFired()
        {
            var changedProperties = new List<string>();
            PropertyChangedEventHandler handler = (s, a) => changedProperties.Add(a.PropertyName);
            this.PropertyChanged += handler;

            RaisePropertyChanged(() => this.InstanceProperty);

            CollectionAssert.AreEqual(new[] { "InstanceProperty" }, changedProperties);

            this.PropertyChanged -= handler;
        }

        [TestMethod]
        public void WhenNotifyingOnAStaticProperty_ThenAnExceptionIsThrown()
        {
            try
            {
                RaisePropertyChanged(() => StaticProperty);
                Assert.Fail("should have thrown");
            }
            catch (ArgumentException)
            {
                // expected
            }
        }

        [TestMethod]
        public void WhenNotifyingOnAInstancePropertyFromADifferentType_ThenAnExceptionIsThrown()
        {
            try
            {
                RaisePropertyChanged(() => "".Length);
                Assert.Fail("should have thrown");
            }
            catch (ArgumentException)
            {
                // expected
            }
        }

        [TestMethod]
        public void WhenNotifyingUsingANullExpression_ThenAnExceptionIsThrown()
        {
            try
            {
                RaisePropertyChanged<int>(null);
                Assert.Fail("should have thrown");
            }
            catch (ArgumentNullException)
            {
                // expected
            }
        }

        [TestMethod]
        public void WhenNotifyingUsingANonMemberAccessExpression_ThenAnExceptionIsThrown()
        {
            try
            {
                RaisePropertyChanged(() => this.GetHashCode());
                Assert.Fail("should have thrown");
            }
            catch (ArgumentException)
            {
                // expected
            }
        }

        [TestMethod]
        public void WhenNotifyingUsingANonPropertyMemberAccessExpression_ThenAnExceptionIsThrown()
        {
            try
            {
                RaisePropertyChanged(() => this.InstanceField);
                Assert.Fail("should have thrown");
            }
            catch (ArgumentException)
            {
                // expected
            }
        }

        [TestMethod]
        [Ignore]    // cannot build the expression
        public void WhenNotifyingUsingAPropertyWithNoGetMethod_ThenAnExceptionIsThrown()
        {
            try
            {
                RaisePropertyChanged(
                    Expression.Lambda<Func<int>>(
                        Expression.MakeMemberAccess(
                            null, 
                            typeof(ViewModelFixture).GetProperty("SetOnlyStaticProperty"))));
                Assert.Fail("should have thrown");
            }
            catch (ArgumentException)
            {
                // expected
            }
        }

        [TestMethod]
        public void WhenSettingErrorsWithNullElements_ThenAnExceptionIsThrown()
        {
            try
            {
                var errors = new List<string>() { "a", null };
                this.SetErrors(()=> this.InstanceProperty, errors);                
                Assert.Fail("should have thrown");
            }
            catch (ArgumentException)
            {
                // expected
            }
        }

        public static int StaticProperty { get; set; }
        public int InstanceProperty { get; set; }
        public int InstanceField;
        public static int SetOnlyStaticProperty { set { } }
    }
}
