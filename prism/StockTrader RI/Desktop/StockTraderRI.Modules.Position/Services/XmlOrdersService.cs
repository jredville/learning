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
using System.IO;
using System.Xml.Linq;
using Microsoft.Practices.Composite.Logging;
using StockTraderRI.Modules.Position.Interfaces;
using StockTraderRI.Modules.Position.Models;
using StockTraderRI.Modules.Position.Properties;

namespace StockTraderRI.Modules.Position.Services
{
    public class XmlOrdersService : IOrdersService
    {
        private ILoggerFacade logger;

        public XmlOrdersService(ILoggerFacade logger)
        {
            this.logger = logger;
        }
        private string _fileName = "SubmittedOrders.xml";

        public string FileName
        {
            get { return _fileName; }
            set { _fileName = value; }
        }

        public void Submit(Order order)
        {
#if !SILVERLIGHT
            XDocument document = File.Exists(FileName) ? XDocument.Load(FileName) : new XDocument();
            Submit(order, document);
            document.Save(FileName);
#else
            // In silverlight, you would normally not save the order to a file, but rather send it to an XML webservice
            // This would be the place were you would call that xml webservice. 
#endif
        }

        public void Submit(Order order, XDocument document)
        {
            var ordersElement = document.Element("Orders");
            if (ordersElement == null)
            {
                ordersElement = new XElement("Orders");
                document.Add(ordersElement);
            }

            var orderElement = new XElement("Order",
                new XAttribute("OrderType", order.OrderType),
                new XAttribute("Shares", order.Shares),
                new XAttribute("StopLimitPrice", order.StopLimitPrice),
                new XAttribute("TickerSymbol", order.TickerSymbol),
                new XAttribute("TimeInForce", order.TimeInForce),
                new XAttribute("TransactionType", order.TransactionType),
                new XAttribute("Date", DateTime.Now.ToString(CultureInfo.InvariantCulture))
                );
            ordersElement.Add(orderElement);

            string message = String.Format(CultureInfo.CurrentCulture, Resources.LogOrderSubmitted,
                                           orderElement.ToString());
            logger.Log(message, Category.Debug, Priority.Low);
        }
    }
}
