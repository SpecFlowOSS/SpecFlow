using System;
using System.Linq;
using System.Reflection;
using System.Text;
using TechTalk.SpecFlow.Bindings;
using TechTalk.SpecFlow.Bindings.Reflection;
using TechTalk.SpecFlow.Infrastructure;

namespace TechTalk.SpecFlow.Tracing
{
    public interface IStepFormatter
    {
        string GetStepDescription(StepInstance stepInstance);
        string GetMatchText(BindingMatch match, object[] arguments);
        string GetMatchText(IBindingMethod method, object[] arguments);
        string GetStepText(StepInstance stepInstance);
    }

    internal class StepFormatter : IStepFormatter
    {
        public const string INDENT = "  ";

        public string GetStepDescription(StepInstance stepInstance)
        {
            return string.Format("{0} {1}", stepInstance.StepDefinitionType, stepInstance.Text);
        }

        public string GetMatchText(BindingMatch match, object[] arguments)
        {
            return GetMatchText(match.StepBinding.Method, arguments);
        }

        public string GetMatchText(IBindingMethod method, object[] arguments)
        {
            string argText = arguments == null ? "" : string.Join(", ", 
                                                          arguments.Select(a => GetParamString(a)).ToArray());
            return string.Format("{0}.{1}({2})", method.Type.Name, method.Name, argText);
        }

        private string GetParamString(object arg)
        {
            const int maxLength = 20;

            if (arg == null)
                return "null";

            if (arg is string)
                return "\"" + arg.ToString().Replace(Environment.NewLine, @"\r\n").TrimEllipse(maxLength) + "\"";

            if (arg is Table)
                return "<table>";

            return arg.ToString().TrimEllipse(maxLength);
        }

        public string GetStepText(StepInstance stepInstance)
        {
            StringBuilder result = new StringBuilder();
            if (stepInstance.Keyword != null)
                result.Append(stepInstance.Keyword);
            else
            {
                result.Append(LanguageHelper.GetDefaultKeyword(stepInstance.StepContext.Language, stepInstance.StepDefinitionKeyword));
                result.Append(" ");
            }
            result.AppendLine(stepInstance.Text);

            if (stepInstance.MultilineTextArgument != null)
            {
                result.AppendLine("--- multiline step argument ---".Indent(INDENT));
                result.AppendLine(stepInstance.MultilineTextArgument.Indent(INDENT));
            }

            if (stepInstance.TableArgument != null)
            {
                result.AppendLine("--- table step argument ---".Indent(INDENT));
                result.AppendLine(stepInstance.TableArgument.ToString().Indent(INDENT));
            }

            return result.ToString();
        }
    }
}