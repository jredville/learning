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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BasicMVVM
{
    public class Questionnaire : IDataErrorInfo
    {
        private readonly QuestionnaireTemplate template;

        public Questionnaire(QuestionnaireTemplate template)
        {
            this.template = template;
            this.Questions = template.Questions.Select(qt => new Question(qt)).ToArray();
        }

        public IEnumerable<Question> Questions { get; private set; }

        public bool IsValid
        {
            get
            {
                return this["Name"] == null && this["Age"] == null && this.Questions.All(q => q.ResponseComplete);
            }
        }

        public string Name { get; set; }

        public int? Age { get; set; }

        public bool IsSubmitted { get; set; }

        public string Error
        {
            get { return null; }
        }

        public string this[string propertyName]
        {
            get
            {
                if (propertyName == "Age")
                {
                    if (!this.Age.HasValue)
                    {
                        return "Age must be completed";
                    }

                    if (this.template.MaxAge.HasValue
                       && (this.template.MaxAge.Value < this.Age || this.template.MinAge > this.Age))
                    {
                        return string.Format("Age should be between {0} and {1}.", this.template.MinAge, this.template.MaxAge.Value);
                    }

                    if (this.template.MinAge > this.Age.Value)
                    {
                        return string.Format("Age should be more than {0}.", this.template.MinAge);
                    }
                }

                if (propertyName == "Name")
                {
                    if (string.IsNullOrEmpty(this.Name))
                    {
                        return "Name should be completed.";
                    }

                    if (this.template.NameMaxLength.HasValue && this.Name.Length > this.template.NameMaxLength.Value)
                    {
                        return string.Format("Name should have less than {0} characters.", this.template.NameMaxLength.Value);
                    }
                }

                return null;
            }
        }
    }
}
