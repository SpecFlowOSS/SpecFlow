using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using EnvDTE;
using EnvDTE80;
using TechTalk.SpecFlow.Bindings;
using TechTalk.SpecFlow.Bindings.Reflection;
using TechTalk.SpecFlow.Vs2010Integration.Utils;

namespace TechTalk.SpecFlow.Vs2010Integration.LanguageService
{
    internal class VsStepSuggestionBindingCollector
    {
        public IEnumerable<StepBinding> GetBindingsFromProjectItem(ProjectItem projectItem)
        {
            return VsxHelper.GetClasses(projectItem).Where(IsBindingClass).SelectMany(GetCompletitionsFromBindingClass);
        }

        private IEnumerable<StepBinding> GetCompletitionsFromBindingClass(CodeClass codeClass)
        {
            var classScopes = codeClass.Attributes.Cast<CodeAttribute2>().Select(GetBingingScopeFromAttribute).Where(s => s != null).ToArray();
            return codeClass.Children.OfType<CodeFunction>().SelectMany(codeFunction => GetSuggestionsFromCodeFunction(codeFunction, classScopes));
        }

        static public bool IsBindingClass(CodeClass codeClass)
        {
            try
            {
                return codeClass.Attributes.Cast<CodeAttribute>().Any(attr => "TechTalk.SpecFlow.BindingAttribute".Equals(attr.FullName));
            }
            catch(Exception)
            {
                return false;
            }
        }

        private IEnumerable<StepBinding> GetSuggestionsFromCodeFunction(CodeFunction codeFunction, IEnumerable<BindingScope> classBindingScopes)
        {
            var bindingScopes = classBindingScopes.Concat(codeFunction.Attributes.Cast<CodeAttribute2>().Select(GetBingingScopeFromAttribute).Where(s => s != null)).ToArray();

            if (bindingScopes.Any())
            {
                foreach (var bindingScope in bindingScopes)
                {
                    foreach (var stepBinding in GetSuggestionsFromCodeFunctionForScope(codeFunction, bindingScope))
                    {
                        yield return stepBinding;
                    }
                }
            }
            else
            {
                foreach (var stepBinding in GetSuggestionsFromCodeFunctionForScope(codeFunction, null))
                {
                    yield return stepBinding;
                }
            }
        }

        private IEnumerable<StepBinding> GetSuggestionsFromCodeFunctionForScope(CodeFunction codeFunction, BindingScope bindingScope)
        {
            return codeFunction.Attributes.Cast<CodeAttribute2>()
                .Select(codeAttribute => GetBingingFromAttribute(codeAttribute, codeFunction, BindingType.Given, bindingScope) ??
                                         GetBingingFromAttribute(codeAttribute, codeFunction, BindingType.When, bindingScope) ??
                                         GetBingingFromAttribute(codeAttribute, codeFunction, BindingType.Then, bindingScope))
                .Where(binding => binding != null);
        }

        private string GetStringArgumentValue(CodeAttribute2 codeAttribute, string argumentName)
        {
            var arg = codeAttribute.Arguments.Cast<CodeAttributeArgument>().FirstOrDefault(a => a.Name == argumentName);
            if (arg == null)
                return null;

            return VsxHelper.ParseCodeStringValue(arg.Value, arg.Language);
        }

        private BindingScope GetBingingScopeFromAttribute(CodeAttribute2 codeAttribute)
        {
            try
            {
                if (codeAttribute.FullName.Equals("TechTalk.SpecFlow.StepScopeAttribute"))
                {
                    var tag = GetStringArgumentValue(codeAttribute, "Tag");
                    string feature = GetStringArgumentValue(codeAttribute, "Feature");
                    string scenario = GetStringArgumentValue(codeAttribute, "Scenario");

                    if (tag == null && feature == null && scenario == null)
                        return null;

                    return new BindingScope(tag, feature, scenario);
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private StepBinding GetBingingFromAttribute(CodeAttribute2 codeAttribute, CodeFunction codeFunction, BindingType bindingType, BindingScope bindingScope)
        {
            try
            {
                if (codeAttribute.FullName.Equals(string.Format("TechTalk.SpecFlow.{0}Attribute", bindingType)))
                    return CreateStepBinding(codeAttribute, codeFunction, bindingType, bindingScope);
                return null;
            }
            catch(Exception)
            {
                return null;
            }
        }

        private StepBinding CreateStepBinding(CodeAttribute2 attr, CodeFunction codeFunction, BindingType bindingType, BindingScope bindingScope)
        {
            try
            {
                IBindingMethod bindingMethod = new VsBindingMethod(codeFunction);

                var regexArg = attr.Arguments.Cast<CodeAttributeArgument>().FirstOrDefault();
                if (regexArg == null)
                    return null;

                var regexString = VsxHelper.ParseCodeStringValue(regexArg.Value, regexArg.Language);
                var regex = new Regex("^" + regexString + "$", RegexOptions.Compiled | RegexOptions.CultureInvariant);

                return new StepBinding(bindingMethod, bindingType, regex, bindingScope);
            }
            catch(Exception)
            {
                return null;
            }
        }

        public CodeFunction FindCodeFunction(VsProjectScope projectScope, IBindingMethod bindingMethod)
        {
            var project = projectScope.Project;

            var function = FindCodeFunction(project, bindingMethod);
            if (function != null)
                return function;

            var specFlowProject = projectScope.SpecFlowProjectConfiguration;
            if (specFlowProject != null)
            {
                foreach (var assemblyName in specFlowProject.RuntimeConfiguration.AdditionalStepAssemblies)
                {
                    string simpleName = assemblyName.Split(new[] { ',' }, 2)[0];

                    var stepProject = VsxHelper.FindProjectByAssemblyName(project.DTE, simpleName);
                    if (stepProject != null)
                    {
                        function = FindCodeFunction(stepProject, bindingMethod);
                        if (function != null)
                            return function;
                    }
                }
            }

            return null;
        }

        private CodeFunction FindCodeFunction(Project project, IBindingMethod bindingMethod)
        {
            return VsxHelper.GetClasses(project).Where(IsBindingClass).Where(c => c.FullName == bindingMethod.Type.FullName)
                .SelectMany(c => c.GetFunctions()).FirstOrDefault(
                    f => f.Name == bindingMethod.Name && BindingReflectionExtensions.MethodEquals(bindingMethod, new VsBindingMethod(f)));
        }
    }
}