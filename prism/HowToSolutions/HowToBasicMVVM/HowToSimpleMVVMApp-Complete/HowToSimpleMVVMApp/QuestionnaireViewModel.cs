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
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.Practices.Composite.Presentation.Commands;
using System.Diagnostics;
using System.Text;

namespace HowToSimpleMVVMApp
{
    public class QuestionnaireViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public QuestionnaireViewModel()
        {
            Question2Items = new List<SelectionItem>
            {
                new SelectionItem { ItemText = "Red" },
                new SelectionItem {ItemText = "Blue" },
                new SelectionItem { ItemText = "Green" }
            };
            SubmitCommand = new DelegateCommand<object>(OnSubmit);
        }
        string _Name;
        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                if (value != _Name)
                {
                    _Name = value;
                    if (PropertyChanged != null)
                        PropertyChanged(this, new PropertyChangedEventArgs("Name"));
                }
            }
        }

        public int Age { get; set; }
        public string Question1ResponseValue { get; set; }
        public List<SelectionItem> Question2Items { get; set; }
        public ICommand SubmitCommand { get; set; }
        private void OnSubmit(object obj)
        {
            Debug.WriteLine(BuildResultString());
        }

        private string BuildResultString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("Name: ");
            builder.Append(Name);
            builder.Append("\nAge: ");
            builder.Append(Age);
            builder.Append("\nQuestion 1: ");
            builder.Append(Question1ResponseValue);
            builder.Append("\nQuestion 2: ");
            foreach (SelectionItem item in Question2Items)
            {
                if (item.ItemChecked)
                {
                    builder.Append("\n");
                    builder.Append(item.ItemText);
                }
            }
            return builder.ToString();
        }

    }

    public class SelectionItem
    {
        public string ItemText { get; set; }
        public bool ItemChecked { get; set; }
    }
}
