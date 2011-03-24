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
using System.Collections.ObjectModel;

namespace BasicMVVM
{
    // QuestionnaireTemplate
    //      [Availability (timeframe)]
    //      [QuestionSet]
    //          QuestionTemplate ( *OpenQuestion, *DiscreteQuestion (e.g. 1-5), *TrueFalseQuestion )
    //              Question
    //              ResponseType
    //              [QuestionTypeTemplate -> QuestionVisual]
    //              
    // 
    // [Survey -> Timeframe/]
    // 
    // Questionnaire[ViewModel?]
    //      DateTaken
    //      PersonTaken
    //      [ResponseSummary -> ]
    //      [QuestionSet]
    //      Questions
    public class QuestionnaireTemplate
    {
        public QuestionnaireTemplate()
        {
            this.Questions = new Collection<QuestionTemplate>();
        }

        public Collection<QuestionTemplate> Questions { get; private set; }

        public int? NameMaxLength { get; set; }

        public int MinAge { get; set; }

        public int? MaxAge { get; set; }
    }
}
