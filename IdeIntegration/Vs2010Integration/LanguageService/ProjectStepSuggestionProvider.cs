using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TechTalk.SpecFlow.Parser.SyntaxElements;

namespace TechTalk.SpecFlow.Vs2010Integration.LanguageService
{
    public class StepInstance
    {
        public FeatureFileInfo FeatureFileInfo { get; private set; }
        public ScenarioStep Step { get; private set; }

        public StepInstance(FeatureFileInfo featureFileInfo, ScenarioStep step)
        {
            FeatureFileInfo = featureFileInfo;
            Step = step;
        }
    }

    public class StepSuggestion
    {
        public readonly List<StepInstance> Instances = new List<StepInstance>();

        public bool HasInstances { get { return Instances.Count > 0; } }

        public string SuggestionText
        {
            get { return HasInstances ? Instances[0].Step.Text : "???"; }
        }

        public override string ToString()
        {
            return SuggestionText;
        }
    }

    public class ProjectStepSuggestionProvider : IDisposable
    {
        private object synchRoot = new object();
        private readonly VsProjectScope vsProjectScope;
        private List<StepSuggestion> stepSuggestions = null;

        public ProjectStepSuggestionProvider(VsProjectScope vsProjectScope)
        {
            this.vsProjectScope = vsProjectScope;
        }

        public void Initialize()
        {
            vsProjectScope.FeatureFilesTracker.Initialized += FeatureFilesTrackerOnInitialized;
            vsProjectScope.FeatureFilesTracker.FeatureFileUpdated += FeatureFilesTrackerOnFeatureFileUpdated;
            vsProjectScope.FeatureFilesTracker.FeatureFileRemoved += FeatureFilesTrackerOnFeatureFileRemoved;
        }

        static private readonly Regex paramRe = new Regex(@"\<(?<param>[^\>]+)\>");

        private IEnumerable<ScenarioStep> GetAllSteps(Feature feature)
        {
            if (feature.Background != null)
                foreach (var scenarioStep in feature.Background.Steps)
                    yield return scenarioStep;

            if (feature.Scenarios != null)
                foreach (var scenario in feature.Scenarios)
                {
                    foreach (var scenarioStep in scenario.Steps)
                        yield return scenarioStep;

                    var scenarioOutline = scenario as ScenarioOutline;
                    if (scenarioOutline != null)
                    {
                        foreach (var scenarioStep in scenario.Steps)
                        {
                            if (!paramRe.Match(scenarioStep.Text).Success)
                                continue;

                            foreach (var exampleSet in scenarioOutline.Examples.ExampleSets)
                            {
                                foreach (var row in exampleSet.Table.Body)
                                {
                                    var replacedText = paramRe.Replace(scenarioStep.Text,
                                        match =>
                                            {
                                                string param = match.Groups["param"].Value;
                                                int headerIndex = Array.FindIndex(exampleSet.Table.Header.Cells,
                                                                    c => c.Value.Equals(param));
                                                if (headerIndex < 0)
                                                    return match.Value;
                                                return row.Cells[headerIndex].Value;
                                            });

                                    if (replacedText.Equals(scenarioStep.Text))
                                        continue;

                                    var newStep = scenarioStep.Clone();
                                    newStep.Text = replacedText;
                                    yield return newStep;
                                }
                            }
                        }
                    }
                }
        }

        private void FeatureFilesTrackerOnInitialized()
        {
            vsProjectScope.VisualStudioTracer.Trace("Building step suggestions...", "ProjectStepSuggestionProvider");

            List<StepSuggestion> newStepSuggestions = new List<StepSuggestion>();
            foreach (var featureFileInfo in vsProjectScope.FeatureFilesTracker.FeatureFiles.Where(ffi => ffi.ParsedFeature != null))
            {
                foreach (var step in GetAllSteps(featureFileInfo.ParsedFeature))
                {
                    var stepSuggestion = new StepSuggestion();
                    stepSuggestion.Instances.Add(new StepInstance(featureFileInfo, step));
                    newStepSuggestions.Add(stepSuggestion);
                }
            }
            lock (synchRoot)
            {
                stepSuggestions = newStepSuggestions;
            }
            vsProjectScope.VisualStudioTracer.Trace("Step suggestions ready", "ProjectStepSuggestionProvider");
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
        }
    }
}