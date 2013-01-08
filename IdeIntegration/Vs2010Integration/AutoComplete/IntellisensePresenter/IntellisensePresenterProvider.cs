using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Language.Intellisense;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Utilities;
using Microsoft.VisualStudio.Shell;
using TechTalk.SpecFlow.Vs2010Integration.AutoComplete.IntellisensePresenter;

namespace Microsoft.VisualStudio.IntellisensePresenter
{
    [Export(typeof(IIntellisensePresenterProvider))]
    [ContentType("gherkin")]
    [Order(Before = "Default Completion Presenter")]
    [Name("Custom Completion Presenter")]
    internal class IntellisensePresenterProvider : IIntellisensePresenterProvider
    {
        [Import(typeof(SVsServiceProvider))]
        private IServiceProvider ServiceProvider { get; set; }

        public IIntellisensePresenter TryCreateIntellisensePresenter(IIntellisenseSession session)
        {
            ICompletionSession completionSession = session as ICompletionSession;
            if (completionSession != null)
            {
                return new CompletionSessionPresenter(ServiceProvider, completionSession);
            }

            return null;
        }
    }
}