using EnvDTE;
using Microsoft.VisualStudio.Text.Editor;
using TechTalk.SpecFlow.Vs2010Integration.LanguageService;
using TechTalk.SpecFlow.Vs2010Integration.Utils;

namespace TechTalk.SpecFlow.Vs2010Integration.EditorCommands
{
    public class GherkinEditorContext
    {
        public GherkinLanguageService LanguageService { get; private set; }
        public IWpfTextView TextView { get; private set; }

        public IProjectScope ProjectScope { get { return LanguageService.ProjectScope; } }

        public GherkinEditorContext(GherkinLanguageService languageService, IWpfTextView textView)
        {
            LanguageService = languageService;
            TextView = textView;
        }

        public static GherkinEditorContext FromDocument(Document document, IGherkinLanguageServiceFactory gherkinLanguageServiceFactory)
        {
            var textView = VsxHelper.GetWpfTextView(VsxHelper.GetIVsTextView(document));
            if (textView == null)
                return null;

            var languageService = gherkinLanguageServiceFactory.GetLanguageService(textView.TextBuffer);
            return new GherkinEditorContext(languageService, textView);
        }
    }
}