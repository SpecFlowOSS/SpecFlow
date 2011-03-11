using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.Language.Intellisense;
using TechTalk.SpecFlow.Parser.SyntaxElements;
using TechTalk.SpecFlow.Vs2010Integration.StepSuggestions;

namespace TechTalk.SpecFlow.Vs2010Integration.LanguageService
{
    public class VsSuggestionItemFactory : INativeSuggestionItemFactory<Completion>
    {
        static public readonly VsSuggestionItemFactory Instance = new VsSuggestionItemFactory();

        public Completion Create(string displayText, string insertionText, int level, object parentObject)
        {
            var result = new Completion(new string(' ', level * 2) + displayText, insertionText, null, null, null);
            if (parentObject != null)
                result.Properties.AddProperty("parentObject", parentObject);

            result.Properties.AddProperty("level", level);
            return result;
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
        private readonly VsStepSuggestionBindingCollector stepSuggestionBindingCollector = new VsStepSuggestionBindingCollector();

        public VsStepSuggestionProvider(VsProjectScope vsProjectScope) : base(VsSuggestionItemFactory.Instance)
        {
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

            AddSuggestionsFromBindings();
            AddSuggestionsFromFeatureFiles();

            Populated = true;
            vsProjectScope.VisualStudioTracer.Trace("Step suggestions ready", "ProjectStepSuggestionProvider");
        }

        private void AddSuggestionsFromBindings()
        {
            foreach (var stepBinding in stepSuggestionBindingCollector.CollectBindingsForSpecFlowProject(vsProjectScope))
            {
                AddBinding(stepBinding);
            }
        }

        private void AddSuggestionsFromFeatureFiles()
        {
            foreach (var featureFileInfo in vsProjectScope.FeatureFilesTracker.FeatureFiles.Where(ffi => ffi.ParsedFeature != null))
            {
                foreach (var suggestion in GetStepSuggestions(featureFileInfo.ParsedFeature))
                {
                    AddStepSuggestion(suggestion);
                }
            }
        }

        private void FeatureFilesTrackerOnFeatureFileRemoved(FeatureFileInfo featureFileInfo)
        {
            //TODO
        }

        private void FeatureFilesTrackerOnFeatureFileUpdated(FeatureFileInfo featureFileInfo)
        {
            //TODO
        }

        public void Dispose()
        {
            vsProjectScope.FeatureFilesTracker.Initialized -= FeatureFilesTrackerOnInitialized;
            vsProjectScope.FeatureFilesTracker.FeatureFileUpdated -= FeatureFilesTrackerOnFeatureFileUpdated;
            vsProjectScope.FeatureFilesTracker.FeatureFileRemoved -= FeatureFilesTrackerOnFeatureFileRemoved;
        }
    }
}