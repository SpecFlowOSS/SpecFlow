using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using EnvDTE;
using EnvDTE80;
using TechTalk.SpecFlow.Vs2010Integration.Bindings;
using TechTalk.SpecFlow.Vs2010Integration.Utils;

namespace TechTalk.SpecFlow.Vs2010Integration.LanguageService
{
    internal class VsStepSuggestionBindingCollector
    {
        public IEnumerable<StepBinding> CollectBindingsForSpecFlowProject(VsProjectScope projectScope)
        {
            var project = projectScope.Project;

            foreach (var stepBinding in GetCompletionsFromProject(project))
            {
                yield return stepBinding;
            }

            var specFlowProject = projectScope.SpecFlowProjectConfiguration;
            if (specFlowProject != null)
            {
                foreach (var assemblyName in specFlowProject.RuntimeConfiguration.AdditionalStepAssemblies)
                {
                    string simpleName = assemblyName.Split(new[] { ',' }, 2)[0];

                    var stepProject = VsxHelper.FindProjectByAssemblyName(project.DTE, simpleName);
                    if (stepProject != null)
                        foreach (var stepBinding in GetCompletionsFromProject(stepProject))
                        {
                            yield return stepBinding;
                        }
                }
            }
        }

        private IEnumerable<StepBinding> GetCompletionsFromProject(Project project)
        {
            return VsxHelper.GetAllProjectItem(project).Where(pi => pi.FileCodeModel != null)
                .SelectMany(projectItem => GetCompletitionsFromCodeElements(projectItem.FileCodeModel.CodeElements));
        }

        private IEnumerable<StepBinding> GetCompletitionsFromCodeElements(CodeElements codeElements)
        {
            foreach (CodeElement codeElement in codeElements)
            {
                if (codeElement.Kind == vsCMElement.vsCMElementFunction)
                {
                    CodeFunction codeFunction = (CodeFunction)codeElement;

                    foreach (var stepBinding in GetSuggestionsFromCodeFunction(codeFunction))
                        yield return stepBinding;
                }
                else if (codeElement.Kind == vsCMElement.vsCMElementClass)
                {
                    CodeClass codeClass = (CodeClass)codeElement;
                    if (codeClass.Attributes.Cast<CodeAttribute>().Any(attr => "TechTalk.SpecFlow.BindingAttribute".Equals(attr.FullName)))
                        foreach (var stepBinding in GetCompletitionsFromCodeElements(codeElement.Children))
                            yield return stepBinding;
                }
                else
                {
                    foreach (var stepBinding in GetCompletitionsFromCodeElements(codeElement.Children))
                        yield return stepBinding;
                }
            }
        }

        private IEnumerable<StepBinding> GetSuggestionsFromCodeFunction(CodeFunction codeFunction)
        {
            return codeFunction.Attributes.Cast<CodeAttribute2>()
                .Select(codeAttribute => GetBingingFromAttribute(codeAttribute, codeFunction, BindingType.Given) ??
                                         GetBingingFromAttribute(codeAttribute, codeFunction, BindingType.When) ??
                                         GetBingingFromAttribute(codeAttribute, codeFunction, BindingType.Then))
                .Where(binding => binding != null);
        }

        private StepBinding GetBingingFromAttribute(CodeAttribute2 codeAttribute, CodeFunction codeFunction, BindingType bindingType)
        {
            if (codeAttribute.FullName.Equals(string.Format("TechTalk.SpecFlow.{0}Attribute", bindingType)))
                return CreateStepBinding(codeAttribute, codeFunction, bindingType);
            return null;
        }

        private StepBinding CreateStepBinding(CodeAttribute2 attr, CodeFunction codeFunction, BindingType bindingType)
        {
            try
            {
                IBindingMethod bindingMethod = new VsBindingMethod(codeFunction);

                var regexArg = attr.Arguments.Cast<CodeAttributeArgument>().FirstOrDefault();
                if (regexArg == null)
                    return null;

                var regexString = VsxHelper.ParseCodeStringValue(regexArg.Value, regexArg.Language);
                var regex = new Regex("^" + regexString + "$", RegexOptions.Compiled | RegexOptions.CultureInvariant);

                return new StepBinding(bindingMethod, bindingType, regex, null); //TODO: handle binding scope
            }
            catch(Exception)
            {
                return null;
            }
        }
    }
}