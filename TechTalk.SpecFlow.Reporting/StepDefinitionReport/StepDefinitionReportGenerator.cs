using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using Gherkin.Ast;
using TechTalk.SpecFlow.Generator.Configuration;
using TechTalk.SpecFlow.Generator.Project;
using TechTalk.SpecFlow.Parser;
using TechTalk.SpecFlow.Reporting.StepDefinitionReport.ReportElements;
using TechTalk.SpecFlow.Tracing;

namespace TechTalk.SpecFlow.Reporting.StepDefinitionReport
{
    public class StepDefinitionReportGenerator
    {
        public StepDefinitionReportParameters ReportParameters { get; set; }
        private readonly SpecFlowProject specFlowProject;
        private readonly List<BindingInfo> bindings;
        private readonly List<SpecFlowDocument> parsedSpecFlowDocuments;

        private ReportElements.StepDefinitionReport report;
        private Dictionary<BindingInfo, StepDefinition> stepDefByBinding;
        private Dictionary<StepDefinition, BindingInfo> bindingByStepDef;
        private readonly List<StepDefinition> stepDefsWithNoBinding = new List<StepDefinition>();

        public StepDefinitionReportGenerator(StepDefinitionReportParameters reportParameters)
        {
            ReportParameters = reportParameters;

            specFlowProject = MsBuildProjectReader.LoadSpecFlowProjectFromMsBuild(reportParameters.ProjectFile);
            parsedSpecFlowDocuments = ParserHelper.GetParsedFeatures(specFlowProject);

            var basePath = Path.Combine(specFlowProject.ProjectSettings.ProjectFolder, reportParameters.BinFolder);
            bindings = BindingCollector.CollectBindings(specFlowProject, basePath);
        }

        public ReportElements.StepDefinitionReport GenerateReport()
        {
            report = new ReportElements.StepDefinitionReport();
            report.ProjectName = specFlowProject.ProjectSettings.ProjectName;
            report.GeneratedAt = DateTime.Now.ToString("g", CultureInfo.InvariantCulture);
            report.ShowBindingsWithoutInsance = ReportParameters.ShowBindingsWithoutInsance;

            stepDefByBinding = new Dictionary<BindingInfo, StepDefinition>();
            bindingByStepDef = new Dictionary<StepDefinition, BindingInfo>();

            foreach (var bindingInfo in bindings)
            {
                var stepDefinition = new StepDefinition();
                stepDefinition.Binding = new Binding { MethodReference = bindingInfo.MethodReference };
                stepDefinition.Type = bindingInfo.BindingType;

                stepDefByBinding.Add(bindingInfo, stepDefinition);
                bindingByStepDef.Add(stepDefinition, bindingInfo);
                report.StepDefinitions.Add(stepDefinition);
            }

            foreach (var specflowDocument in parsedSpecFlowDocuments)
            {
                var feature = specflowDocument.SpecFlowFeature;
                var featureRef = new FeatureRef { FilePath = specflowDocument.SourceFilePath, Name = feature.Name };
                if (feature.Background != null)
                {
                    var scenarioRef = new ScenarioRef { Name = "Background" };
                    AddStepInstances(featureRef, scenarioRef, feature.Background.Steps, false);
                }

                foreach (var scenario in feature.ScenarioDefinitions)
                {
                    var scenarioRef = new ScenarioRef { Name = scenario.Name, SourceFileLine = scenario.Location == null ? -1 : scenario.Location.Line };
                    if (scenario is ScenarioOutline)
                    {
                        var firstExampleSteps = CreateFirstExampleScenarioSteps((ScenarioOutline) scenario);
                        AddStepInstances(featureRef, scenarioRef, firstExampleSteps, true);
                    }
                    else
                    {
                        AddStepInstances(featureRef, scenarioRef, scenario.Steps, false);
                    }
                }
            }

            foreach (var stepDefinition in report.StepDefinitions)
            {
                if (stepDefinition.ScenarioStep == null)
                {
                    CreateSampleStep(stepDefinition);
                }
                
                if (stepDefinition.Instances.Count == 0)
                {
                    stepDefinition.Instances = null;
                }

                //if(stepDefinition.ScenarioStep.ScenarioBlock == 0)
                //{
                //    stepDefinition.ScenarioStep.ScenarioBlock = (Parser.ScenarioBlock)Enum.Parse(typeof (Parser.ScenarioBlock), stepDefinition.Type);
                //}

                //if(stepDefinition.ScenarioStep.StepKeyword == 0)
                //{
                //    stepDefinition.ScenarioStep.StepKeyword = (StepKeyword)Enum.Parse(typeof(StepKeyword), stepDefinition.Type);
                //}
            }

            return report;
        }

        private IEnumerable<SpecFlowStep> CreateFirstExampleScenarioSteps(ScenarioOutline scenarioOutline)
        {
            foreach (var exampleSet in scenarioOutline.Examples)
            {
                foreach (var example in exampleSet.TableBody)
                {
                    Dictionary<string, string> paramSubst = new Dictionary<string, string>();
                    for (int i = 0; i < exampleSet.TableHeader.Cells.Count(); i++)
                    {
                        paramSubst.Add(exampleSet.TableHeader.Cells.ElementAt(i).Value, example.Cells.ElementAt(i).Value);
                    }

                    return CreateScenarioSteps(scenarioOutline, paramSubst);
                }
            }
            return new List<SpecFlowStep>();
        }

        private IEnumerable<SpecFlowStep> CreateScenarioSteps(ScenarioOutline scenarioOutline, Dictionary<string, string> paramSubst)
        {
            var result = new List<SpecFlowStep>();
            foreach (var scenarioStep in scenarioOutline.Steps)
            {
                var specflowStep = (SpecFlowStep) scenarioStep;

                var stepArgument = specflowStep.Argument;
                var dataTable = specflowStep.Argument as DataTable;

                if (dataTable != null)
                {
                    stepArgument = Clone(dataTable, (c) => GetReplacedText(c.Value, paramSubst));
                }

                var newStep = Clone(specflowStep, stepArgument);                

                result.Add(newStep);
            }
            return result;
        }

        private string GetReplacedText(string text, Dictionary<string, string> paramSubst)
        {
            if (text == null || paramSubst == null)
                return text;

            foreach (var subst in paramSubst)
            {
                text = text.Replace("<" + subst.Key + ">", subst.Value);
            }

            return text;
        }

        private void CreateSampleStep(StepDefinition stepDefinition)
        {
            BindingInfo bindingInfo = bindingByStepDef[stepDefinition];
            Debug.Assert(bindingInfo != null);
            var sampleText = GetSampleText(bindingInfo);

            switch (stepDefinition.Type)
            {
                case "Given":
                    stepDefinition.ScenarioStep = new ReportStep(SpecFlowLocation.Empty, "Given", sampleText, null, StepKeyword.Given, Parser.ScenarioBlock.Given);
                    break;
                case "When":
                    stepDefinition.ScenarioStep = new ReportStep(SpecFlowLocation.Empty, "When", sampleText, null, StepKeyword.When, Parser.ScenarioBlock.When);
                    break;
                case "Then":
                    stepDefinition.ScenarioStep = new ReportStep(SpecFlowLocation.Empty, "Then", sampleText, null, StepKeyword.Then, Parser.ScenarioBlock.Then);
                    break;
                default:
                    throw new InvalidOperationException();
            }

        }

        private string GetSampleText(BindingInfo bindingInfo, string text, Match match)
        {
            StringBuilder sampleText = new StringBuilder(text);
            for (int groupIndex = match.Groups.Count - 1; groupIndex >= 1; groupIndex--)
            {
                int paramIndex = groupIndex - 1;
                var paramText = paramIndex >= bindingInfo.ParameterNames.Length ? "{?param?}" : "{" + bindingInfo.ParameterNames[paramIndex] + "}";

                var gr = match.Groups[groupIndex];
                sampleText.Remove(gr.Index, gr.Length);
                sampleText.Insert(gr.Index, paramText);
            }
            return sampleText.ToString();
        }

        private string GetSampleText(BindingInfo bindingInfo)
        {
            var sampleText = bindingInfo.Regex.ToString().Trim('$', '^');
            Regex re = new Regex(@"\([^\)]+\)");
            int paramIndex = 0;
            sampleText = re.Replace(sampleText, delegate
                                                    {
                                                        return paramIndex >= bindingInfo.ParameterNames.Length ? "{?param?}" :
                                                                                                                                 "{" + bindingInfo.ParameterNames[paramIndex++] + "}";
                                                    });
            return sampleText;
        }

        public void TransformReport()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(ReportElements.StepDefinitionReport), ReportElements.StepDefinitionReport.XmlNamespace);

            if (XsltHelper.IsXmlOutput(ReportParameters.OutputFile))
            {
                XsltHelper.TransformXml(serializer, report, ReportParameters.OutputFile);
            }
            else
            {
                XsltHelper.TransformHtml(serializer, report, GetType(), ReportParameters.OutputFile, specFlowProject.Configuration.SpecFlowConfiguration, ReportParameters.XsltFile);
            }
        }

        private void AddStepInstances(FeatureRef featureRef, ScenarioRef scenarioRef, IEnumerable<Step> scenarioSteps, bool fromScenarioOutline)
        {
            var specFlowSteps = scenarioSteps.Cast<SpecFlowStep>();

            foreach (var scenarioStep in specFlowSteps)
            {
                string currentBlock = scenarioStep.ScenarioBlock.ToString();                  

                bool found = false;

                foreach (var bindingInfo in bindings)
                {
                    if (bindingInfo.BindingType != currentBlock)
                        continue;

                    var match = bindingInfo.Regex.Match(scenarioStep.Text);
                    if (!match.Success)
                        continue;

                    Instance instance = new Instance
                                            {
                                                FromScenarioOutline = fromScenarioOutline,
                                                ScenarioStep = CloneTo(scenarioStep, scenarioStep.ScenarioBlock.ToString(), scenarioStep.Text),
                                                FeatureRef = featureRef,
                                                ScenarioRef = scenarioRef
                                            };
                    var regexArgs = match.Groups.Cast<Group>().Skip(1).Select(g => g.Value).ToArray();
                    if (regexArgs.Length > 0)
                    {
                        instance.Parameters = new List<Parameter>();
                        for (int i = 0; i < Math.Min(regexArgs.Length, bindingInfo.ParameterNames.Length); i++)
                            instance.Parameters.Add(new Parameter { Name = bindingInfo.ParameterNames[i], Value = regexArgs[i] });
                    }

                    var stepDefinition = stepDefByBinding[bindingInfo];
                    stepDefinition.Instances.Add(instance);

                    if (stepDefinition.ScenarioStep == null)
                    {
                        var sampleText = GetSampleText(bindingInfo, scenarioStep.Text, match);
                        stepDefinition.ScenarioStep = CloneTo(scenarioStep, currentBlock, sampleText);
                        
                    }

                    found = true;
                    break;
                }

                if (!found)
                {
                    var stepDefinition =
                        stepDefsWithNoBinding.FirstOrDefault(sd => sd.ScenarioStep.Text.Equals(scenarioStep.Text));

                    if (stepDefinition == null)
                    {
                        stepDefinition = new StepDefinition();
                        stepDefinition.Type = currentBlock;
                        stepDefinition.ScenarioStep = CloneTo(scenarioStep, currentBlock, scenarioStep.Text);
                        stepDefsWithNoBinding.Add(stepDefinition);
                        report.StepDefinitions.Add(stepDefinition);
                    }

                    Instance instance = new Instance
                                            {
                                                FromScenarioOutline = fromScenarioOutline,
                                                ScenarioStep = CloneTo(scenarioStep, scenarioStep.ScenarioBlock.ToString(), scenarioStep.Text),
                                                FeatureRef = featureRef,
                                                ScenarioRef = scenarioRef
                                            };
                    stepDefinition.Instances.Add(instance);
                }
            }
        }

        private SpecFlowStep Clone(SpecFlowStep step, StepArgument stepArgument)
        {
            return new SpecFlowStep(step.Location, step.Keyword, step.Text, stepArgument, step.StepKeyword, step.ScenarioBlock);
        }

        private DataTable Clone(DataTable table, Func<TableCell, string> getCellValue = null)
        {
            if (table == null)
                return null;

            return new DataTable(table.Rows.Select(r => Clone(r, getCellValue)).ToArray());
        }

        private Gherkin.Ast.TableRow Clone(Gherkin.Ast.TableRow row, Func<TableCell, string> getValue = null)
        {
            var valueExpr = getValue ?? ((c) => c.Value);

            return new Gherkin.Ast.TableRow(row.Location, row.Cells.Select(c => new TableCell(c.Location, valueExpr(c))).ToArray());
        }

        private ReportStep CloneTo(SpecFlowStep step, string currentBlock, string sampleText)
        {
            ReportStep newStep = null;
            if (currentBlock == "When")
                newStep = new ReportStep(step.Location, step.Keyword, sampleText, ConvertArgument(step.Argument), step.StepKeyword, Parser.ScenarioBlock.When);
            else if (currentBlock == "Then")
                newStep = new ReportStep(step.Location, step.Keyword, sampleText, ConvertArgument(step.Argument), step.StepKeyword, Parser.ScenarioBlock.Then);
            else // Given or empty
                newStep = new ReportStep(step.Location, step.Keyword, sampleText, ConvertArgument(step.Argument), step.StepKeyword, Parser.ScenarioBlock.Given);

            Debug.Assert(newStep != null);
            
            return newStep;
        }

        private ReportStepArgument ConvertArgument(StepArgument argument)
        {
            if (argument == null)
                return null;

            if (argument is DocString)
            {
                DocString arg = (DocString) argument;

                return new DocStringArgument(arg.ContentType, arg.Content);
            }

            if (argument is DataTable)
            {
                DataTable arg = (DataTable) argument;
                
                return new TableArgument(arg.Rows.Select(r => new ReportTableRow(r.Cells.Select(c => new ReportTableCell(c.Value)).ToList())).ToList());
            }

            return null;
        }


        public void GenerateAndTransformReport()
        {
            GenerateReport();
            TransformReport();
        }
    }
}