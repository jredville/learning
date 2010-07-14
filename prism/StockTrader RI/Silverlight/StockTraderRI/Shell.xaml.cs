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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using StockTraderRI.Resources;

namespace StockTraderRI
{
    public partial class Shell : UserControl, IShellView
    {
        private const int VisibleGridDefaultHeight = 5;

        public Shell()
        {
            InitializeComponent();
        }

        public void ShowView()
        {
            Application.Current.RootVisual = this;
            var story = (Storyboard)this.Resources[ResourceNames.EntryStoryboardName];
            story.Begin();
        }

        private void ActionControl_LayoutUpdated(object sender, System.EventArgs e)
        {
            Visibility newVisibility = (this.ActionControl.Content != null) ? Visibility.Visible : Visibility.Collapsed;
            if (this.ActionControl.Visibility != newVisibility)
            {
                this.ActionControl.Visibility = newVisibility;
                this.ActionRow.Height = new GridLength(newVisibility == Visibility.Visible ? VisibleGridDefaultHeight : 0, GridUnitType.Star);
            }
        }
    }
}
