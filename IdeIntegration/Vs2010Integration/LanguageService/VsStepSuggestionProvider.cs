using System;
using System.Globalization;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.VisualStudio.Language.Intellisense;
using TechTalk.SpecFlow.Infrastructure;
using TechTalk.SpecFlow.Parser.SyntaxElements;
using TechTalk.SpecFlow.Bindings;
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

    public class VsStepSuggestionProvider : StepSuggestionProvider<Completion>, IDisposable, IBindingRegistry
    {
        private bool featureFilesPopulated = false;
        private bool bindingsPopulated = false;
        private readonly VsProjectScope vsProjectScope;

        private bool readyInvoked = false;
        public event Action Ready;
        public event Action BindingsChanged;

        private readonly Dictionary<BindingFileInfo, List<IStepDefinitionBinding>> bindingSuggestions = new Dictionary<BindingFileInfo, List<IStepDefinitionBinding>>();
        private readonly Dictionary<FeatureFileInfo, List<IStepSuggestion<Completion>>> fileSuggestions = new Dictionary<FeatureFileInfo, List<IStepSuggestion<Completion>>>();

        public bool Populated
        {
            get { return featureFilesPopulated && bindingsPopulated; }
        }

        public bool FeatureFilesPopulated
        {
            get { return featureFilesPopulated; }
        }

        public bool BindingsPopulated
        {
            get { return bindingsPopulated; }
        }

        bool IBindingRegistry.Ready
        {
            get { return BindingsPopulated; }
            set { /*nop*/ }
        }

        protected override IStepDefinitionMatchService BindingMatchService
        {
            get { return vsProjectScope.BindingMatchService; }
        }

        public VsStepSuggestionProvider(VsProjectScope vsProjectScope)
            : base(VsSuggestionItemFactory.Instance, vsProjectScope)
        {
            this.vsProjectScope = vsProjectScope;
        }

        public int GetPopulationPercent()
        {
            if (Populated)
                return 100;
            if (!vsProjectScope.FeatureFilesTracker.IsInitialized || !vsProjectScope.BindingFilesTracker.IsInitialized)
                return 0;
            var totalCount = vsProjectScope.FeatureFilesTracker.Files.Count() + vsProjectScope.BindingFilesTracker.Files.Count();
            if (totalCount == 0)
                return 100;
            return ((fileSuggestions.Count + bindingSuggestions.Count)*100)/totalCount;
        }

        public void Initialize()
        {
            vsProjectScope.FeatureFilesTracker.Ready += FeatureFilesTrackerOnReady;
            vsProjectScope.FeatureFilesTracker.FileUpdated += FeatureFilesTrackerOnFeatureFileUpdated;
            vsProjectScope.FeatureFilesTracker.FileRemoved += FeatureFilesTrackerOnFeatureFileRemoved;

            vsProjectScope.BindingFilesTracker.Ready += BindingFilesTrackerOnReady;
            vsProjectScope.BindingFilesTracker.FileUpdated += BindingFilesTrackerOnFileUpdated;
            vsProjectScope.BindingFilesTracker.FileRemoved += BindingFilesTrackerOnFileRemoved;
        }

        private StepContext CreateStepScope(Feature feature, Scenario scenario)
        {
            var tags =
                (feature.Tags.AsEnumerable() ?? Enumerable.Empty<Tag>())
                .Concat(scenario.Tags.AsEnumerable() ?? Enumerable.Empty<Tag>())
                .Select(t => t.Name).Distinct();
            return new StepContext(feature.Title, scenario.Title, tags.ToArray(), GetLanguage(feature));
        }

        private StepContext CreateStepScope(Feature feature)
        {
            var tags = (feature.Tags.AsEnumerable() ?? Enumerable.Empty<Tag>())
                .Select(t => t.Name).Distinct();
            return new StepContext(feature.Title, null, tags.ToArray(), GetLanguage(feature));
        }

        private CultureInfo GetLanguage(Feature feature)
        {
            var language = this.vsProjectScope.GherkinDialectServices.GetGherkinDialect(feature).CultureInfo;
            return language;
        }

        private IEnumerable<IStepSuggestion<Completion>> GetStepSuggestions(Feature feature)
        {
            if (feature.Background != null)
            {
                var featureScope = CreateStepScope(feature);
                foreach (var scenarioStep in feature.Background.Steps)
                    yield return new StepInstance<Completion>(scenarioStep, feature, featureScope, nativeSuggestionItemFactory);
            }

            if (feature.Scenarios != null)
                foreach (var scenario in feature.Scenarios)
                {
                    var scenarioOutline = scenario as ScenarioOutline;
                    var stepScope = CreateStepScope(feature, scenario);
                    foreach (var scenarioStep in scenario.Steps)
                    {
                        if (scenarioOutline == null || !StepInstanceTemplate<Completion>.IsTemplate(scenarioStep))
                        {
                            yield return new StepInstance<Completion>(scenarioStep, feature, stepScope, nativeSuggestionItemFactory);
                        }
                        else
                        {
                            yield return new StepInstanceTemplate<Completion>(scenarioStep, scenarioOutline, feature, stepScope, nativeSuggestionItemFactory);
                        }
                    }
                }
        }

        private void FeatureFilesTrackerOnReady()
        {
            featureFilesPopulated = true;
            vsProjectScope.Tracer.Trace("Suggestions from feature files ready", "ProjectStepSuggestionProvider");

            FireReady();
        }

        private void FireReady()
        {
            if (Populated && !readyInvoked && Ready != null)
            {
                bool doReady;
                lock(this)
                {
                    doReady = !readyInvoked;
                    readyInvoked = true;
                }

                if (doReady)
                    Ready();
            }
        }

        private void FireBindingsChanged()
        {
            if (Populated && BindingsChanged != null)
            {
                BindingsChanged();
            }
        }

        private void BindingFilesTrackerOnReady()
        {
            bindingsPopulated = true;
            vsProjectScope.Tracer.Trace("Suggestions from bindings ready", "ProjectStepSuggestionProvider");
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
            
            if (featureFileInfo.ParsedFeature != null)
            {
                if (stepSuggestions == null)
                {
                    stepSuggestions = new List<IStepSuggestion<Completion>>();
                    fileSuggestions.Add(featureFileInfo, stepSuggestions);
                }

                stepSuggestions.AddRange(GetStepSuggestions(featureFileInfo.ParsedFeature));
                stepSuggestions.ForEach(AddStepSuggestion);
            }
        }

        private void BindingFilesTrackerOnFileRemoved(BindingFileInfo bindingFileInfo)
        {
            List<IStepDefinitionBinding> bindings;
            if (bindingSuggestions.TryGetValue(bindingFileInfo, out bindings))
            {
                bindings.ForEach(RemoveBinding);
                bindingSuggestions.Remove(bindingFileInfo);

                FireBindingsChanged();
            }
        }

        private void BindingFilesTrackerOnFileUpdated(BindingFileInfo bindingFileInfo)
        {
            bool isChanged = false;

            List<IStepDefinitionBinding> bindings;
            if (bindingSuggestions.TryGetValue(bindingFileInfo, out bindings))
            {
                bindings.ForEach(RemoveBinding);
                bindings.Clear();
                isChanged = true;
            }

            if (!bindingFileInfo.IsError && bindingFileInfo.StepBindings.Any())
            {
                if (bindings == null)
                {
                    bindings = new List<IStepDefinitionBinding>();
                    bindingSuggestions.Add(bindingFileInfo, bindings);
                }

                bindings.AddRange(bindingFileInfo.StepBindings);
                bindings.ForEach(AddBinding);
                isChanged = true;
            }

            if (isChanged)
                FireBindingsChanged();
        }

        public void RegisterStepDefinitionBinding(IStepDefinitionBinding stepDefinitionBinding)
        {
            throw new NotSupportedException();
        }

        public void RegisterHookBinding(IHookBinding hookBinding)
        {
            throw new NotSupportedException();
        }

        public void RegisterStepArgumentTransformationBinding(IStepArgumentTransformationBinding stepArgumentTransformationBinding)
        {
            throw new NotSupportedException();
        }

        public IEnumerable<IHookBinding> GetHooks(HookType bindingEvent)
        {
            return Enumerable.Empty<IHookBinding>(); //not used in VS
        }

        public IEnumerable<IStepArgumentTransformationBinding> GetStepTransformations()
        {
            return Enumerable.Empty<IStepArgumentTransformationBinding>(); //not used in VS
        }

        public void Dispose()
        {
            vsProjectScope.FeatureFilesTracker.Ready -= FeatureFilesTrackerOnReady;
            vsProjectScope.FeatureFilesTracker.FileUpdated -= FeatureFilesTrackerOnFeatureFileUpdated;
            vsProjectScope.FeatureFilesTracker.FileRemoved -= FeatureFilesTrackerOnFeatureFileRemoved;

            vsProjectScope.BindingFilesTracker.Ready -= BindingFilesTrackerOnReady;
            vsProjectScope.BindingFilesTracker.FileUpdated -= BindingFilesTrackerOnFileUpdated;
            vsProjectScope.BindingFilesTracker.FileRemoved -= BindingFilesTrackerOnFileRemoved;
        }
    }
}