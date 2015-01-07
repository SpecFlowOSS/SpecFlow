using EnvDTE;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;
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
	        var vsTextView = VsxHelper.GetIVsTextView(document);

	        return FromVsTextView(gherkinLanguageServiceFactory, vsTextView);
        }

	    public static GherkinEditorContext FromVsTextView(IGherkinLanguageServiceFactory gherkinLanguageServiceFactory,
		    IVsTextView vsTextView)
	    {
		    var textView = VsxHelper.GetWpfTextView(vsTextView);
		    if (textView == null)
			    return null;

		    var languageService = gherkinLanguageServiceFactory.GetLanguageService(textView.TextBuffer);
		    return new GherkinEditorContext(languageService, textView);
	    }
    }
}