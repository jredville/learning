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
using StockTraderRI.Modules.Position.Interfaces;
using StockTraderRI.Modules.Position.PositionSummary;

namespace StockTraderRI.Modules.Position.Tests.PositionSummary
{
    [TestClass]
    public class PositionPieChartPresentationModelFixture
    {
        [TestMethod]
        public void ShouldBuildCorrectly()
        {
            var view = new MockPositionPieChartView();
            var observablePosition = new MockObservablePosition();

            PositionPieChartPresentationModel model = new PositionPieChartPresentationModel(view, observablePosition);

            Assert.AreSame(view, model.View);
            Assert.AreSame(observablePosition, model.Position);
        }

        private class MockPositionPieChartView : IPositionPieChartView
        {
            public IPositionPieChartPresentationModel Model { get; set; }
        }
    }
}
