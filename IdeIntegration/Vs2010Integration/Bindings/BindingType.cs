using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using TechTalk.SpecFlow.Parser.Gherkin;

namespace TechTalk.SpecFlow.Vs2010Integration.Bindings
{
    public enum BindingType
    {
        Given = ScenarioBlock.Given,
        When = ScenarioBlock.When,
        Then = ScenarioBlock.Then
    }

    public interface IBindingType
    {
        string Name { get; }
        string FullName { get; }
    }

    public interface IBindingParameter
    {
        IBindingType Type { get; }
        string ParameterName { get; }
    }

    public interface IBindingMethod
    {
        IBindingType Type { get; }
        string Name { get; }
        IEnumerable<IBindingParameter> Parameters { get; }
        string ShortDisplayText { get; }
    }

    public class StepBinding
    {
        public IBindingMethod Method { get; private set; }
        public BindingType BindingType { get; private set; }
        public Regex Regex { get; private set; }

        public BindingScope BindingScope { get; private set; }
        public bool IsScoped { get { return BindingScope != null; } }

        public StepBinding(IBindingMethod method, BindingType bindingType, Regex regex, BindingScope bindingScope)
        {
            Method = method;
            BindingType = bindingType;
            Regex = regex;
            BindingScope = bindingScope;
        }
    }

    public class BindingScope
    {
        public string Tag { get; private set; }
        public string FeatureTitle { get; private set; }
        public string ScenarioTitle { get; private set; }

        public BindingScope(string tag, string featureTitle, string scenarioTitle)
        {
            Tag = tag;
            FeatureTitle = featureTitle;
            ScenarioTitle = scenarioTitle;
        }
    }


}
