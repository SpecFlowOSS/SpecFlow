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
        string GetStepDescription(StepArgs stepArgs);
        string GetMatchText(BindingMatch match, object[] arguments);
        string GetMatchText(IBindingMethod method, object[] arguments);
        string GetStepText(StepArgs stepArgs);
    }

    internal class StepFormatter : IStepFormatter
    {
        public const string INDENT = "  ";

        public string GetStepDescription(StepArgs stepArgs)
        {
            return string.Format("{0} {1}", stepArgs.Type, stepArgs.Text);
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

        public string GetStepText(StepArgs stepArgs)
        {
            StringBuilder result = new StringBuilder();
            result.Append(stepArgs.OriginalStepKeyword ?? 
                LanguageHelper.GetDefaultKeyword(stepArgs.StepContext.Language, stepArgs.StepDefinitionKeyword));
            result.Append(" ");
            result.AppendLine(stepArgs.Text);

            if (stepArgs.MultilineTextArgument != null)
            {
                result.AppendLine("--- multiline step argument ---".Indent(INDENT));
                result.AppendLine(stepArgs.MultilineTextArgument.Indent(INDENT));
            }

            if (stepArgs.TableArgument != null)
            {
                result.AppendLine("--- table step argument ---".Indent(INDENT));
                result.AppendLine(stepArgs.TableArgument.ToString().Indent(INDENT));
            }

            return result.ToString();
        }
    }
}