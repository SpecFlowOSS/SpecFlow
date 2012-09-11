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
            var stepTransformation = GetMatchingStepTransformation(value, typeToConvertTo, true);

            if (stepTransformation == null)
                throw new SpecFlowException("The StepTransformationConverter cannot convert the specified value.");

            return DoTransform(stepTransformation, value, cultureInfo);
        }

        public bool CanConvert(object value, IBindingType typeToConvertTo, CultureInfo cultureInfo)
        {
            return GetMatchingStepTransformation(value, typeToConvertTo, false) != null;
        }

        private IStepArgumentTransformationBinding GetMatchingStepTransformation(object value, IBindingType typeToConvertTo, bool traceWarning)
        {
            var stepTransformations = bindingRegistry.GetStepTransformations().Where(t => CanConvert(t, value, typeToConvertTo)).ToArray();

            if (stepTransformations.Length > 1 && traceWarning)
            {
                testTracer.TraceWarning(string.Format("Multiple step transformation matches to the input ({0}, target type: {1}). We use the first.", value, typeToConvertTo));
            }

            return stepTransformations.FirstOrDefault();
        }

        private bool CanConvert(IStepArgumentTransformationBinding stepTransformationBinding, object value, IBindingType typeToConvertTo)
        {
            if (!stepTransformationBinding.Method.ReturnType.TypeEquals(typeToConvertTo))
                return false;

            if (stepTransformationBinding.Regex != null)
            {
                return value is string && stepTransformationBinding.Regex.IsMatch((string)value);
            }

            return stepTransformationBinding.Method.Parameters.Count() == 1;
        }

        private object DoTransform(IStepArgumentTransformationBinding stepTransformation, object value, CultureInfo cultureInfo)
        {
            object[] arguments;
            if (stepTransformation.Regex != null && value is string)
                arguments = GetStepTransformationArgumentsFromRegex(stepTransformation, (string)value, cultureInfo);
            else
                arguments = new object[] { value };

            TimeSpan duration;
            return bindingInvoker.InvokeBinding(stepTransformation, contextManager, arguments, testTracer, out duration);
        }

        private object[] GetStepTransformationArgumentsFromRegex(IStepArgumentTransformationBinding stepTransformation, string stepSnippet, CultureInfo cultureInfo)
        {
            var match = stepTransformation.Regex.Match(stepSnippet);
            var argumentStrings = match.Groups.Cast<Group>().Skip(1).Select(g => g.Value);
            var bindingParameters = stepTransformation.Method.Parameters.ToArray();
            return argumentStrings
                .Select((arg, argIndex) => stepArgumentTypeConverter.Convert(arg, bindingParameters[argIndex].Type, cultureInfo))
                .ToArray();
        }
    }
}
