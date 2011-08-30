using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TechTalk.SpecFlow.Bindings;
using TechTalk.SpecFlow.Generator.Project;
using TechTalk.SpecFlow.Parser.SyntaxElements;

namespace TechTalk.SpecFlow.Vs2010Integration.SkeletonHelpers
{
    public class BindingsAnalyser
    {
        public List<BindingInfo> Bindings { get; private set; }
        private readonly SpecFlowProject _specFlowProject;
        private readonly Feature _feature;

        public BindingsAnalyser(SpecFlowProject specFlowProject, Feature feature, string binFolder)
        {
            _specFlowProject = specFlowProject;
            _feature = feature;
            var basePath = Path.Combine(_specFlowProject.ProjectSettings.ProjectFolder, binFolder);
            Bindings = BindingCollector.CollectBindings(_specFlowProject, basePath);
        }

        public List<StepInstance> GetMissingSteps()
        {
            List<StepInstance> missingSteps = new List<StepInstance>();
            foreach (var scenario in _feature.Scenarios)
            {
                foreach (var step in scenario.Steps)
                {
                    bool stepDefExists = false;
                    foreach (var binding in Bindings)
                    {
                        stepDefExists = binding.Regex.IsMatch(step.Text);
                        if (stepDefExists)
                            break;
                    }
                    if (!stepDefExists)
                    {
                        StepInstance stepInstance = ConvertToStepInstance(step, scenario.Title);
                        missingSteps.Add(stepInstance);
                    }
                }
            }
            return missingSteps;
        }

        private StepInstance ConvertToStepInstance(ScenarioStep scenarioStep, string scenarioTitle)
        {
            StepInstance stepInstance;
            IEnumerable<string> tags = null;
            if (_feature.Tags != null)
                tags = _feature.Tags.Select(
                                                tag => tag.ToString());
            StepScopeNew scope = new StepScopeNew(_feature.Title, scenarioTitle, tags);
            if (scenarioStep.ScenarioBlock == Parser.Gherkin.ScenarioBlock.Given)
                stepInstance = new StepInstance(BindingType.Given, StepDefinitionKeyword.Given, scenarioStep.Keyword,
                                            scenarioStep.Text, scope);
            else if (scenarioStep.ScenarioBlock == Parser.Gherkin.ScenarioBlock.When)
                stepInstance = new StepInstance(BindingType.When, StepDefinitionKeyword.When, scenarioStep.Keyword,
                                            scenarioStep.Text, scope);
            else if (scenarioStep.ScenarioBlock == Parser.Gherkin.ScenarioBlock.Then)
                stepInstance = new StepInstance(BindingType.Then, StepDefinitionKeyword.Then, scenarioStep.Keyword,
                                            scenarioStep.Text, scope);
            else throw new SpecFlowException();
            stepInstance.MultilineTextArgument = scenarioStep.MultiLineTextArgument;
            if (scenarioStep.TableArg != null)
                //HACK: The tables are of different type, and I see no availble way of converting between types. 
                // The only time they are used in the skeletons is to check if they are not null and add a table
                //argument if that is the case, so I set it to an arbritrary non null value for now.
                stepInstance.TableArgument = new Table("Not", "Null");

            return stepInstance;
        }
    }
}
