using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using TechTalk.SpecFlow.Bindings.Reflection;
using TechTalk.SpecFlow.Infrastructure;
using TechTalk.SpecFlow.Tracing;

namespace TechTalk.SpecFlow.Bindings
{
    internal class StepArgumentTransformationConverter : IStepArgumentTypeConverter
    {
        private readonly IStepArgumentTypeConverter stepArgumentTypeConverter;
        private readonly ITestTracer testTracer;
        private readonly IBindingRegistry bindingRegistry;
        private readonly IContextManager contextManager;
        private readonly IBindingInvoker bindingInvoker;

        public StepArgumentTransformationConverter(IStepArgumentTypeConverter stepArgumentTypeConverter, ITestTracer testTracer, IBindingRegistry bindingRegistry, IContextManager contextManager, IBindingInvoker bindingInvoker)
        {
            this.stepArgumentTypeConverter = stepArgumentTypeConverter;
            this.testTracer = testTracer;
            this.bindingRegistry = bindingRegistry;
            this.contextManager = contextManager;
            this.bindingInvoker = bindingInvoker;
        }

        public object Convert(object value, IBindingType typeToConvertTo, CultureInfo cultureInfo)
        {
            var stepTransformation = GetMatchingStepTransformation(value, typeToConvertTo, cultureInfo, true);

            if (stepTransformation == null)
                throw new SpecFlowException("The StepTransformationConverter cannot convert the specified value.");

            return DoTransform(stepTransformation, value, cultureInfo);
        }

        public bool CanConvert(object value, IBindingType typeToConvertTo, CultureInfo cultureInfo)
        {
            return GetMatchingStepTransformation(value, typeToConvertTo, cultureInfo, false) != null;
        }

        private IStepArgumentTransformationBinding GetMatchingStepTransformation(object value, IBindingType typeToConvertTo, CultureInfo cultureInfo, bool traceWarning)
        {
            var stepTransformations = bindingRegistry.GetStepTransformations().Where(t => CanConvert(t, value, typeToConvertTo, cultureInfo)).ToArray();

            if (stepTransformations.Length > 1 && traceWarning)
            {
                testTracer.TraceWarning(string.Format("Multiple step transformation matches to the input ({0}, target type: {1}). We use the first.", value, typeToConvertTo));
            }

            return stepTransformations.FirstOrDefault();
        }

        private bool CanConvert(IStepArgumentTransformationBinding stepTransformationBinding, object value, IBindingType typeToConvertTo, CultureInfo cultureInfo)
        {
            if (!stepTransformationBinding.Method.ReturnType.TypeEquals(typeToConvertTo))
                return false;

            if (stepTransformationBinding.Regex != null && !(value is string))
            {
                return false;
            }

            var arguments = GetStepTransformationArguments(stepTransformationBinding, value, cultureInfo);
            var parameters = stepTransformationBinding.Method.Parameters.ToArray();

            if (arguments == null || arguments.Length != parameters.Length)
                return false;

            for (var i = 0; i < parameters.Length; i++)
            {
                if (!stepArgumentTypeConverter.CanConvert(arguments[i], parameters[i].Type, cultureInfo))
                    return false;
            }

            return true;
        }

        private object DoTransform(IStepArgumentTransformationBinding stepTransformationBinding, object value, CultureInfo cultureInfo)
        {
            var parameters = stepTransformationBinding.Method.Parameters.ToArray();
            var arguments = GetStepTransformationArguments(stepTransformationBinding, value, cultureInfo)
                .Select((arg, i) => stepArgumentTypeConverter.Convert(arg, parameters[i].Type, cultureInfo)).ToArray();

            TimeSpan duration;
            return bindingInvoker.InvokeBinding(stepTransformationBinding, contextManager, arguments, testTracer, out duration);
        }

        private object[] GetStepTransformationArguments(IStepArgumentTransformationBinding stepTransformation, object value, CultureInfo cultureInfo)
        {
            if (stepTransformation.Regex != null && value is string)
                return GetStepTransformationArgumentsFromRegex(stepTransformation, (string) value, cultureInfo);

            return new[] {value};
        }

        private object[] GetStepTransformationArgumentsFromRegex(IStepArgumentTransformationBinding stepTransformation, string stepSnippet, CultureInfo cultureInfo)
        {
            var match = stepTransformation.Regex.Match(stepSnippet);

            if (!match.Success)
            {
                return null;
            }

            return match.Groups.Cast<Group>().Skip(1).Select(g => g.Value).Cast<object>().ToArray();
        }
    }
}
