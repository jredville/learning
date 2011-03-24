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
using System.Linq;

namespace BasicMVVM.Model
{
    public class MultipleSelectionQuestionTemplate : QuestionTemplate
    {
        public MultipleSelectionQuestionTemplate()
        {
            this.Range = new List<string>();
        }

        public IList<string> Range { get; private set; }

        public int? MaxSelections { get; set; }

        public override bool IsValid(Question question)
        {
            var selections = question.Response as IEnumerable<string>;

            if (selections == null)
            {
                return false;
            }

            var selectionsArray = selections.ToArray();
            if (this.MaxSelections.HasValue && selectionsArray.Length > this.MaxSelections.Value)
            {
                return false;
            }

            return selectionsArray.All(s => this.Range.Contains(s));
        }
    }
}
