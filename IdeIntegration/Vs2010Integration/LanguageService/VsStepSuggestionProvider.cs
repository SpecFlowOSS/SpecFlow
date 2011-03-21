using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.VisualStudio.Language.Intellisense;
using TechTalk.SpecFlow.Parser.SyntaxElements;
using TechTalk.SpecFlow.Vs2010Integration.Bindings;
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
        public bool Populated
        {
            get { return featureFilesPopulated && bindingsPopulated; }
        }
        private bool featureFilesPopulated = false;
        private bool bindingsPopulated = false;
        private readonly VsProjectScope vsProjectScope;
        private readonly VsStepSuggestionBindingCollector stepSuggestionBindingCollector;

        public VsStepSuggestionProvider(VsProjectScope vsProjectScope) : base(VsSuggestionItemFactory.Instance)
        {
            stepSuggestionBindingCollector = new VsStepSuggestionBindingCollector(vsProjectScope.VisualStudioTracer);
            this.vsProjectScope = vsProjectScope;
        }

        public void Initialize()
        {
            vsProjectScope.FeatureFilesTracker.Initialized += FeatureFilesTrackerOnInitialized;
            vsProjectScope.FeatureFilesTracker.FileUpdated += FeatureFilesTrackerOnFeatureFileUpdated;
            vsProjectScope.FeatureFilesTracker.FileRemoved += FeatureFilesTrackerOnFeatureFileRemoved;

            vsProjectScope.BindingFilesTracker.Initialized += BindingFilesTrackerOnInitialized;
            vsProjectScope.BindingFilesTracker.FileUpdated += BindingFilesTrackerOnFileUpdated;
            vsProjectScope.BindingFilesTracker.FileRemoved += BindingFilesTrackerOnFileRemoved;
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
//            vsProjectScope.VisualStudioTracer.Trace("Processing step instances...", "ProjectStepSuggestionProvider");
//            AddSuggestionsFromFeatureFiles();

            featureFilesPopulated = true;
            vsProjectScope.VisualStudioTracer.Trace("Suggestions from feature files ready", "ProjectStepSuggestionProvider");
        }

        private void BindingFilesTrackerOnInitialized()
        {
//            vsProjectScope.VisualStudioTracer.Trace("Processing bindings...", "ProjectStepSuggestionProvider");
//            AddSuggestionsFromBindings();

            bindingsPopulated = true;
            vsProjectScope.VisualStudioTracer.Trace("Suggestions from bindings ready", "ProjectStepSuggestionProvider");
        }

        private void AddSuggestionsFromBindings()
        {
            foreach (var bindingFileInfo in vsProjectScope.BindingFilesTracker.Files)
            {
                var bindings = bindingFileInfo.StepBindings.ToList();
                bindingSuggestions.Add(bindingFileInfo, bindings);
                bindings.ForEach(AddBinding);
            }
        }

        private readonly Dictionary<FeatureFileInfo, List<IStepSuggestion<Completion>>> fileSuggestions = new Dictionary<FeatureFileInfo, List<IStepSuggestion<Completion>>>();

        private void AddSuggestionsFromFeatureFiles()
        {
            foreach (var featureFileInfo in vsProjectScope.FeatureFilesTracker.Files.Where(ffi => ffi.ParsedFeature != null))
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

        private readonly Dictionary<BindingFileInfo, List<StepBinding>> bindingSuggestions = new Dictionary<BindingFileInfo, List<StepBinding>>();

        private void BindingFilesTrackerOnFileRemoved(BindingFileInfo bindingFileInfo)
        {
            List<StepBinding> bindings;
            if (bindingSuggestions.TryGetValue(bindingFileInfo, out bindings))
            {
                bindings.ForEach(RemoveBinding);
                bindingSuggestions.Remove(bindingFileInfo);
            }
        }

        private void BindingFilesTrackerOnFileUpdated(BindingFileInfo bindingFileInfo)
        {
            List<StepBinding> bindings;
            if (bindingSuggestions.TryGetValue(bindingFileInfo, out bindings))
            {
                bindings.ForEach(RemoveBinding);
                bindings.Clear();
            }
            else
            {
                bindings = new List<StepBinding>();
                bindingSuggestions.Add(bindingFileInfo, bindings);
            }

            if (bindingFileInfo.StepBindings.Any())
            {
                bindings.AddRange(bindingFileInfo.StepBindings);
                bindings.ForEach(AddBinding);
            }
        }

        public void Dispose()
        {
            vsProjectScope.FeatureFilesTracker.Initialized -= FeatureFilesTrackerOnInitialized;
            vsProjectScope.FeatureFilesTracker.FileUpdated -= FeatureFilesTrackerOnFeatureFileUpdated;
            vsProjectScope.FeatureFilesTracker.FileRemoved -= FeatureFilesTrackerOnFeatureFileRemoved;
        }
    }
}