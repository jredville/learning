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
using System.Windows.Controls;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace StockTraderRI.Infrastructure.Tests
{
    [TestClass]
    public class TreeHelperFixture
    {
        [TestMethod]
        public void ShouldFindDirectAncestor()
        {
            ContentControl parent = new ContentControl();
            TextBlock child = new TextBlock();
            parent.Content = child;

            var foundParent = TreeHelper.FindAncestor(child, d => d == parent);

            Assert.IsNotNull(foundParent);
            Assert.AreSame(parent, foundParent);
        }

        [TestMethod]
        public void ShouldFindIndirectAncestor()
        {
            ContentControl grandParent = new ContentControl();
            ContentControl parent = new ContentControl();
            TextBlock child = new TextBlock();
            grandParent.Content = parent;
            parent.Content = child;

            var foundGrandParent = TreeHelper.FindAncestor(child, d => d == grandParent);

            Assert.IsNotNull(foundGrandParent);
            Assert.AreSame(grandParent, foundGrandParent);
        }
    }
}
