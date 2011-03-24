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
using System.Linq;
using System.Text;
using StockTraderRI.Infrastructure.Interfaces;
using StockTraderRI.Infrastructure.Models;

namespace StockTraderRI.Modules.Position.Tests.Mocks
{
    class MockAccountPositionService : IAccountPositionService
    {
        List<AccountPosition> positionData = new List<AccountPosition>();

        public void AddPosition(string ticker, decimal costBasis, long shares)
        {
            AddPosition(new AccountPosition(ticker, costBasis, shares));
        }

        public void AddPosition(AccountPosition position)
        {
            position.Updated += new EventHandler<AccountPositionEventArgs>(position_Updated);
            positionData.Add(position);

            //Notify that collection has changed
            Updated(this, new AccountPositionModelEventArgs(position));
        }

        void position_Updated(object sender, AccountPositionEventArgs e)
        {
            Updated(this, new AccountPositionModelEventArgs(sender as AccountPosition));
        }


        #region IAccountPositionService Members

        public IList<AccountPosition> GetAccountPositions()
        {
            return positionData;
        }
        
        public event EventHandler<AccountPositionModelEventArgs> Updated = delegate { };

        #endregion
    }

  
}
