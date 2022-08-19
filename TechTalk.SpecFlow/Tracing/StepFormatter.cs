using System;
using System.Linq;
using System.Text;
using TechTalk.SpecFlow.Bindings;
using TechTalk.SpecFlow.Bindings.Reflection;

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
        private readonly IColorOutputHelper colorOutputHelper;
        private readonly IColorOutputTheme colorOutputTheme;

        public StepFormatter(
            IColorOutputHelper colorOutputHelper,
            IColorOutputTheme colorOutputTheme
        )
        {
            this.colorOutputHelper = colorOutputHelper;
            this.colorOutputTheme = colorOutputTheme;
        }

        public const string INDENT = "  ";

        public string GetStepDescription(StepInstance stepInstance)
        {
            return $"{stepInstance.StepDefinitionType} {stepInstance.Text}";
        }

        public string GetMatchText(BindingMatch match, object[] arguments)
        {
            return GetMatchText(match.StepBinding.Method, arguments);
        }

        public string GetMatchText(IBindingMethod method, object[] arguments)
        {
            string argText = arguments == null ? "" : string.Join(", ", arguments.Select(a => GetParamString(a)).ToArray());
            return $"{method.Type.Name}.{method.Name}({argText})";
        }

        private string GetParamString(object arg)
        {
            const int maxLength = 20;

            return arg switch
            {
                null => "null",
                string => "\"" + arg.ToString().Replace(Environment.NewLine, @"\r\n").TrimEllipse(maxLength) + "\"",
                Table => "<table>",
                _ => arg.ToString().TrimEllipse(maxLength)
            };
        }

        public string GetStepText(StepInstance stepInstance)
        {
            StringBuilder result = new StringBuilder();
            if (stepInstance.Keyword != null)
                result.Append(colorOutputHelper.Colorize(stepInstance.Keyword, colorOutputTheme.Keyword));
            else
            {
                result.Append(colorOutputHelper.Colorize(LanguageHelper.GetDefaultKeyword(stepInstance.StepContext.Language, stepInstance.StepDefinitionKeyword), colorOutputTheme.Keyword));
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