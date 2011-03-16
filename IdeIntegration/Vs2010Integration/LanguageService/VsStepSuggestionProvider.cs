using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Microsoft.VisualStudio.Language.Intellisense;
using TechTalk.SpecFlow.Parser.SyntaxElements;
using TechTalk.SpecFlow.Vs2010Integration.StepSuggestions;

namespace TechTalk.SpecFlow.Vs2010Integration.LanguageService
{
    internal class CompletionWithImage : Completion
    {
        public CompletionWithImage(string displayText, string insertionText, string description, ImageSource iconSource, string iconAutomationText) : base(displayText, insertionText, description, iconSource, iconAutomationText)
        {
        }

        public string IconDescriptor { get; set; }

        public override ImageSource IconSource
        {
            get
            {
                if (base.IconSource == null && IconDescriptor != null)
                {
                    base.IconSource = new BitmapImage(
                        new Uri(string.Format("pack://application:,,,/TechTalk.SpecFlow.Vs2010Integration;component/Resources/autocomplete-{0}.png", 
                            IconDescriptor.ToLowerInvariant())));
                }

                return base.IconSource;
            }
            set
            {
                base.IconSource = value;
            }
        }
    }

    public class VsSuggestionItemFactory : INativeSuggestionItemFactory<Completion>
    {
        static public readonly VsSuggestionItemFactory Instance = new VsSuggestionItemFactory();

        public Completion Create(string displayText, string insertionText, int level, string iconDescriptor, object parentObject)
        {
            var result = new CompletionWithImage(new string(' ', level*2) + displayText, insertionText, null, null, null) {IconDescriptor = iconDescriptor};
            if (parentObject != null)
                result.Properties.AddProperty("parentObject", parentObject);

            result.Properties.AddProperty("level", level);
            return result;
        }

        public Completion CloneTo(Completion nativeSuggestionItem, object parentObject)
        {
            return Create(nativeSuggestionItem.DisplayText.TrimStart(), nativeSuggestionItem.InsertionText,
                          GetLevel(nativeSuggestionItem), ((CompletionWithImage) nativeSuggestionItem).IconDescriptor,
                          parentObject);
        }

        public string GetInsertionText(Completion nativeSuggestionItem)
        {
            return nativeSuggestionItem.InsertionText;
        }

        public int GetLevel(Completion nativeSuggestionItem)
        {
            return nativeSuggestionItem.Properties.GetProperty<int>("level");
        }
    }

    public class VsStepSuggestionProvider : StepSuggestionProvider<Completion>, IDisposable
    {
        public bool Populated { get; private set; }
        private readonly VsProjectScope vsProjectScope;
        private readonly VsStepSuggestionBindingCollector stepSuggestionBindingCollector;

        public VsStepSuggestionProvider(VsProjectScope vsProjectScope) : base(VsSuggestionItemFactory.Instance)
        {
            stepSuggestionBindingCollector = new VsStepSuggestionBindingCollector(vsProjectScope.VisualStudioTracer);
            this.vsProjectScope = vsProjectScope;
            Populated = false;
        }

        public void Initialize()
        {
            vsProjectScope.FeatureFilesTracker.Initialized += FeatureFilesTrackerOnInitialized;
            vsProjectScope.FeatureFilesTracker.FeatureFileUpdated += FeatureFilesTrackerOnFeatureFileUpdated;
            vsProjectScope.FeatureFilesTracker.FeatureFileRemoved += FeatureFilesTrackerOnFeatureFileRemoved;
        }

        private IEnumerable<IStepSuggestion<Completion>> GetStepSuggestions(Feature feature)
        {
            if (feature.Background != null)
                foreach (var scenarioStep in feature.Background.Steps)
                    yield return new StepInstance<Completion>(scenarioStep, null, feature, nativeSuggestionItemFactory);

            if (feature.Scenarios != null)
                foreach (var scenario in feature.Scenarios)
                {
                    var scenarioOutline = scenario as ScenarioOutline;
                    foreach (var scenarioStep in scenario.Steps)
                    {
                        if (scenarioOutline == null || !StepInstanceTemplate<Completion>.IsTemplate(scenarioStep))
                        {
                            yield return new StepInstance<Completion>(scenarioStep, scenario, feature, nativeSuggestionItemFactory);
                        }
                        else
                        {
                            yield return new StepInstanceTemplate<Completion>(scenarioStep, scenarioOutline, feature, nativeSuggestionItemFactory);
                        }
                    }
                }
        }

        private void FeatureFilesTrackerOnInitialized()
        {
            vsProjectScope.VisualStudioTracer.Trace("Building step suggestions...", "ProjectStepSuggestionProvider");

            vsProjectScope.VisualStudioTracer.Trace("Processing bindings...", "ProjectStepSuggestionProvider");
            AddSuggestionsFromBindings();

            vsProjectScope.VisualStudioTracer.Trace("Processing step instances...", "ProjectStepSuggestionProvider");
            AddSuggestionsFromFeatureFiles();

            Populated = true;
            vsProjectScope.VisualStudioTracer.Trace("Step suggestions ready", "ProjectStepSuggestionProvider");
            if (vsProjectScope.VisualStudioTracer.IsEnabled("ProjectStepSuggestionProvider"))  // bypass calculating statistics if trace is not enabled
                vsProjectScope.VisualStudioTracer.Trace("Statistics:" + boundStepSuggestions.GetStatistics(), "ProjectStepSuggestionProvider");
        }

        private void AddSuggestionsFromBindings()
        {
            foreach (var stepBinding in stepSuggestionBindingCollector.CollectBindingsForSpecFlowProject(vsProjectScope))
            {
                AddBinding(stepBinding);
            }
        }

        Dictionary<FeatureFileInfo, List<IStepSuggestion<Completion>>> fileSuggestions = new Dictionary<FeatureFileInfo, List<IStepSuggestion<Completion>>>();

        private void AddSuggestionsFromFeatureFiles()
        {
            foreach (var featureFileInfo in vsProjectScope.FeatureFilesTracker.FeatureFiles.Where(ffi => ffi.ParsedFeature != null))
            {
                var stepSuggestions = GetStepSuggestions(featureFileInfo.ParsedFeature).ToList();
                fileSuggestions.Add(featureFileInfo, stepSuggestions);
                stepSuggestions.ForEach(AddStepSuggestion);
            }
        }

        private void FeatureFilesTrackerOnFeatureFileRemoved(FeatureFileInfo featureFileInfo)
        {
            List<IStepSuggestion<Completion>> stepSuggestions;
            if (fileSuggestions.TryGetValue(featureFileInfo, out stepSuggestions))
            {
                stepSuggestions.ForEach(RemoveStepSuggestion);
                fileSuggestions.Remove(featureFileInfo);
            }
        }

        private void FeatureFilesTrackerOnFeatureFileUpdated(FeatureFileInfo featureFileInfo)
        {
            List<IStepSuggestion<Completion>> stepSuggestions;
            if (fileSuggestions.TryGetValue(featureFileInfo, out stepSuggestions))
            {
                stepSuggestions.ForEach(RemoveStepSuggestion);
                stepSuggestions.Clear();
            }
            else
            {
                stepSuggestions = new List<IStepSuggestion<Completion>>();
                fileSuggestions.Add(featureFileInfo, stepSuggestions);
            }
            
            if (featureFileInfo.ParsedFeature != null)
            {
                stepSuggestions.AddRange(GetStepSuggestions(featureFileInfo.ParsedFeature));
                stepSuggestions.ForEach(AddStepSuggestion);
            }
        }

        public void Dispose()
        {
            vsProjectScope.FeatureFilesTracker.Initialized -= FeatureFilesTrackerOnInitialized;
            vsProjectScope.FeatureFilesTracker.FeatureFileUpdated -= FeatureFilesTrackerOnFeatureFileUpdated;
            vsProjectScope.FeatureFilesTracker.FeatureFileRemoved -= FeatureFilesTrackerOnFeatureFileRemoved;
        }
    }
}