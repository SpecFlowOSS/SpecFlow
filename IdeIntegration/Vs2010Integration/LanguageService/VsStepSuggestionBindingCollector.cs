using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using EnvDTE;
using EnvDTE80;
using TechTalk.SpecFlow.Vs2010Integration.Bindings;
using TechTalk.SpecFlow.Vs2010Integration.Tracing;
using TechTalk.SpecFlow.Vs2010Integration.Utils;

namespace TechTalk.SpecFlow.Vs2010Integration.LanguageService
{
    internal class VsStepSuggestionBindingCollector
    {
        private readonly IVisualStudioTracer visualStudioTracer;

        public VsStepSuggestionBindingCollector(IVisualStudioTracer visualStudioTracer)
        {
            this.visualStudioTracer = visualStudioTracer;
        }

        public IEnumerable<StepBinding> GetBindingsFromProjectItem(ProjectItem projectItem)
        {
            return VsxHelper.GetClasses(projectItem).Where(IsBindingClass).SelectMany(GetCompletitionsFromBindingClass);
        }

        private IEnumerable<StepBinding> GetCompletitionsFromBindingClass(CodeClass codeClass)
        {
            //visualStudioTracer.Trace("Analyzing binding class: " + codeClass.FullName, "ProjectStepSuggestionProvider");
            return codeClass.Children.OfType<CodeFunction>().SelectMany(GetSuggestionsFromCodeFunction);
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
            try
            {
                if (codeAttribute.FullName.Equals(string.Format("TechTalk.SpecFlow.{0}Attribute", bindingType)))
                    return CreateStepBinding(codeAttribute, codeFunction, bindingType);
                return null;
            }
            catch(Exception)
            {
                return null;
            }
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
                    f => f.Name == bindingMethod.Name && MethodEquals(bindingMethod, new VsBindingMethod(f)));
        }

        private bool MethodEquals(IBindingMethod method1, IBindingMethod method2)
        {
            return method1.Name == method2.Name; //TODO: complete
        }
    }
}