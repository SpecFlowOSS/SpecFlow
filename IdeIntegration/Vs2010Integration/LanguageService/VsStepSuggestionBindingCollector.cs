﻿using System;
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
        public IEnumerable<StepBindingNew> GetBindingsFromProjectItem(ProjectItem projectItem)
        {
            foreach (CodeClass bindingClassWithBindingAttribute in VsxHelper.GetClasses(projectItem).Where(IsBindingClass))
            {
                CodeClass2 bindingClassIncludingParts = bindingClassWithBindingAttribute as CodeClass2;

                BindingScopeNew[] bindingScopes = GetClassScopes(bindingClassWithBindingAttribute);
                foreach (StepBindingNew currrentFoundStep in GetStepsFromClass(bindingClassWithBindingAttribute, bindingScopes))
                {
                    yield return currrentFoundStep;
                }

                foreach (CodeClass2 currentBindingPartialClass in bindingClassIncludingParts.Parts)
                {
                    foreach (StepBindingNew currentPartialClassStep in GetStepsFromClass(currentBindingPartialClass as CodeClass, bindingScopes))
                    {
                        yield return currentPartialClassStep;
                    }
                }
            }
        }

        private BindingScopeNew[] GetClassScopes(CodeClass codeClass)
        {
            return codeClass.Attributes.Cast<CodeAttribute2>().Select(GetBingingScopeFromAttribute).Where(s => s != null).ToArray();
        }

        private IEnumerable<StepBindingNew> GetStepsFromClass(CodeClass codeClass, BindingScopeNew[] classScopes)
        {
            return codeClass.Children.OfType<CodeFunction>().SelectMany(codeFunction => GetSuggestionsFromCodeFunction(codeFunction, classScopes));
        }

        static public bool IsBindingClass(CodeClass codeClass)
        {
            try
            {
                return codeClass.Attributes.Cast<CodeAttribute>().Any(attr => typeof(BindingAttribute).FullName.Equals(attr.FullName));
            }
            catch(Exception)
            {
                return false;
            }
        }

        private IEnumerable<StepBindingNew> GetSuggestionsFromCodeFunction(CodeFunction codeFunction, IEnumerable<BindingScopeNew> classBindingScopes)
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

        private IEnumerable<StepBindingNew> GetSuggestionsFromCodeFunctionForScope(CodeFunction codeFunction, BindingScopeNew bindingScope)
        {
            return codeFunction.Attributes.Cast<CodeAttribute2>()
                .SelectMany(codeAttribute => GetStepDefinitionsFromAttribute(codeAttribute, codeFunction, bindingScope))
                .Where(binding => binding != null);
        }

        private string GetStringArgumentValue(CodeAttribute2 codeAttribute, string argumentName)
        {
            var arg = codeAttribute.Arguments.Cast<CodeAttributeArgument>().FirstOrDefault(a => a.Name == argumentName);
            if (arg == null)
                return null;

            return VsxHelper.ParseCodeStringValue(arg.Value, arg.Language);
        }

        private BindingScopeNew GetBingingScopeFromAttribute(CodeAttribute2 codeAttribute)
        {
            try
            {
                if (IsScopeAttribute(codeAttribute))
                {
                    var tag = GetStringArgumentValue(codeAttribute, "Tag");
                    string feature = GetStringArgumentValue(codeAttribute, "Feature");
                    string scenario = GetStringArgumentValue(codeAttribute, "Scenario");

                    if (tag == null && feature == null && scenario == null)
                        return null;

                    return new BindingScopeNew(tag, feature, scenario);
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private bool IsScopeAttribute(CodeAttribute2 codeAttribute)
        {
            return 
                codeAttribute.FullName.Equals(typeof(ScopeAttribute).FullName) ||
#pragma warning disable 612,618
                codeAttribute.FullName.Equals(typeof(StepScopeAttribute).FullName);
#pragma warning restore 612,618
        }

        private IEnumerable<StepBindingNew> GetStepDefinitionsFromAttribute(CodeAttribute2 codeAttribute, CodeFunction codeFunction, BindingScopeNew bindingScope)
        {
            var normalStepDefinition =
                GetBingingFromAttribute(codeAttribute, codeFunction, BindingType.Given, bindingScope) ??
                GetBingingFromAttribute(codeAttribute, codeFunction, BindingType.When, bindingScope) ??
                GetBingingFromAttribute(codeAttribute, codeFunction, BindingType.Then, bindingScope);
            if (normalStepDefinition != null)
            {
                yield return normalStepDefinition;
                yield break;
            }

            if (IsGeneralStepDefinition(codeAttribute))
            {
                yield return CreateStepBinding(codeAttribute, codeFunction, BindingType.Given, bindingScope);
                yield return CreateStepBinding(codeAttribute, codeFunction, BindingType.When, bindingScope);
                yield return CreateStepBinding(codeAttribute, codeFunction, BindingType.Then, bindingScope);
            }
        }

        private StepBindingNew GetBingingFromAttribute(CodeAttribute2 codeAttribute, CodeFunction codeFunction, BindingType bindingType, BindingScopeNew bindingScope)
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

        private bool IsGeneralStepDefinition(CodeAttribute2 codeAttribute)
        {
            try
            {
                return codeAttribute.FullName.Equals(typeof (StepDefinitionAttribute).FullName);
            }
            catch(Exception)
            {
                return false;
            }
        }

        private StepBindingNew CreateStepBinding(CodeAttribute2 attr, CodeFunction codeFunction, BindingType bindingType, BindingScopeNew bindingScope)
        {
            try
            {
                IBindingMethod bindingMethod = new VsBindingMethod(codeFunction);

                var regexArg = attr.Arguments.Cast<CodeAttributeArgument>().FirstOrDefault();
                if (regexArg == null)
                    return null;

                var regexString = VsxHelper.ParseCodeStringValue(regexArg.Value, regexArg.Language);
                var regex = new Regex("^" + regexString + "$", RegexOptions.Compiled | RegexOptions.CultureInvariant);

                return new StepBindingNew(bindingMethod, bindingType, regex, bindingScope);
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
            return GetBindingClassesIncludingPartialClasses(project)
                .Where(c => c.FullName == bindingMethod.Type.FullName)
                .SelectMany(c => c.GetFunctions()).FirstOrDefault(
                f => f.Name == bindingMethod.Name && BindingReflectionExtensions.MethodEquals(bindingMethod, new VsBindingMethod(f)));
        }

        private IEnumerable<CodeClass> GetBindingClassesIncludingPartialClasses(Project project)
        {
            foreach (CodeClass bindingClassWithBindingAttribute in VsxHelper.GetClasses(project).Where(IsBindingClass))
            {
                yield return bindingClassWithBindingAttribute;

                CodeClass2 bindingClassIncludingParts = bindingClassWithBindingAttribute as CodeClass2;
                foreach (CodeClass2 currentBindingPartialClass in bindingClassIncludingParts.Parts)
                {
                    yield return currentBindingPartialClass as CodeClass;
                }
            }
        }
    }
}