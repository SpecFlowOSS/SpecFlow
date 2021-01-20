using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using TechTalk.SpecFlow.Assist.ValueRetrievers;
using TechTalk.SpecFlow.Bindings.Reflection;
using TechTalk.SpecFlow.Infrastructure;
using TechTalk.SpecFlow.Tracing;

namespace TechTalk.SpecFlow.Bindings
{
    public interface IStepArgumentTypeConverter
    {
        object Convert(object value, IBindingType typeToConvertTo, CultureInfo cultureInfo);
        bool CanConvert(object value, IBindingType typeToConvertTo, CultureInfo cultureInfo);
    }

    public class StepArgumentTypeConverter : IStepArgumentTypeConverter
    {
        private readonly ITestTracer testTracer;
        private readonly IBindingRegistry bindingRegistry;
        private readonly IContextManager contextManager;
        private readonly IBindingInvoker bindingInvoker;

        public StepArgumentTypeConverter(ITestTracer testTracer, IBindingRegistry bindingRegistry, IContextManager contextManager, IBindingInvoker bindingInvoker)
        {
            this.testTracer = testTracer;
            this.bindingRegistry = bindingRegistry;
            this.contextManager = contextManager;
            this.bindingInvoker = bindingInvoker;
        }

        protected virtual IStepArgumentTransformationBinding GetMatchingStepTransformation(object value, IBindingType typeToConvertTo, bool traceWarning)
        {
            var stepTransformations = bindingRegistry.GetStepTransformations().Where(t => CanConvert(t, value, typeToConvertTo)).ToArray();
            if (stepTransformations.Length > 1 && traceWarning)
            {
                testTracer.TraceWarning(string.Format("Multiple step transformation matches to the input ({0}, target type: {1}). We use the first.", value, typeToConvertTo));
            }

            return stepTransformations.Length > 0 ? stepTransformations[0] : null;
        }

        public object Convert(object value, IBindingType typeToConvertTo, CultureInfo cultureInfo)
        {
            if (value == null) throw new ArgumentNullException("value");
            
            var stepTransformation = GetMatchingStepTransformation(value, typeToConvertTo, true);
            if (stepTransformation != null)
                return DoTransform(stepTransformation, value, cultureInfo);

            var convertToType = typeToConvertTo as RuntimeBindingType;
            if (convertToType != null && convertToType.Type.IsAssignableFrom(value.GetType()))
                return value;

            return ConvertSimple(typeToConvertTo, value, cultureInfo);
        }

        private object DoTransform(IStepArgumentTransformationBinding stepTransformation, object value, CultureInfo cultureInfo)
        {
            object[] arguments;
            if (stepTransformation.Regex != null && value is string)
                arguments = GetStepTransformationArgumentsFromRegex(stepTransformation, (string)value, cultureInfo);
            else
                arguments = new object[] {value};

            TimeSpan duration;
            return bindingInvoker.InvokeBinding(stepTransformation, contextManager, arguments, testTracer, out duration);
        }

        private object[] GetStepTransformationArgumentsFromRegex(IStepArgumentTransformationBinding stepTransformation, string stepSnippet, CultureInfo cultureInfo)
        {
            var match = stepTransformation.Regex.Match(stepSnippet);
            var argumentStrings = match.Groups.Cast<Group>().Skip(1).Select(g => g.Value);
            var bindingParameters = stepTransformation.Method.Parameters.ToArray();
            return argumentStrings
                .Select((arg, argIndex) => this.Convert(arg, bindingParameters[argIndex].Type, cultureInfo))
                .ToArray();
        }

        public bool CanConvert(object value, IBindingType typeToConvertTo, CultureInfo cultureInfo)
        {
            if (value == null) throw new ArgumentNullException("value");           

            var stepTransformation = GetMatchingStepTransformation(value, typeToConvertTo, false);
            if (stepTransformation != null)
                return true;

            var convertToType = typeToConvertTo as RuntimeBindingType;
            if (convertToType != null && convertToType.Type.IsAssignableFrom(value.GetType()))
                return true;

            return CanConvertSimple(typeToConvertTo, value, cultureInfo);
        }

        private bool CanConvert(IStepArgumentTransformationBinding stepTransformationBinding, object valueToConvert, IBindingType typeToConvertTo)
        {
            if (!stepTransformationBinding.Method.ReturnType.TypeEquals(typeToConvertTo))
                return false;

            if (stepTransformationBinding.Regex != null && valueToConvert is string)
                return stepTransformationBinding.Regex.IsMatch((string) valueToConvert);
            
            var transformationFirstArgumentTypeName = stepTransformationBinding.Method.Parameters.FirstOrDefault()?.Type.FullName;
            
            var isTableStepTransformation = transformationFirstArgumentTypeName == typeof(Table).FullName;
            var valueIsTable = valueToConvert is Table;
            
            return isTableStepTransformation == valueIsTable;
        }

        private static object ConvertSimple(IBindingType typeToConvertTo, object value, CultureInfo cultureInfo)
        {
            if (!(typeToConvertTo is RuntimeBindingType))
                throw new SpecFlowException("The StepArgumentTypeConverter can be used with runtime types only.");

            return ConvertSimple(((RuntimeBindingType) typeToConvertTo).Type, value, cultureInfo);
        }

        private static object ConvertSimple(Type typeToConvertTo, object value, CultureInfo cultureInfo)
        {
            if (typeToConvertTo.IsEnum && value is string)
                return ConvertToAnEnum(typeToConvertTo, (string) value);

            if (typeToConvertTo == typeof(Guid?) && string.IsNullOrEmpty(value as string))
                return null;

            if (typeToConvertTo == typeof(Guid) || typeToConvertTo == typeof(Guid?))
                return new GuidValueRetriever().GetValue(value as string);

            return TryConvertWithTypeConverter(typeToConvertTo, value, cultureInfo, out var convertedValue) 
                ? convertedValue : 
                System.Convert.ChangeType(value, typeToConvertTo, cultureInfo);
        }

        private static bool TryConvertWithTypeConverter(Type typeToConvertTo, object value, CultureInfo cultureInfo, out object result)
        {
            var typeConverter = TypeDescriptor.GetConverter(typeToConvertTo);
            
            if (typeConverter.CanConvertFrom(value.GetType()))
            {
                try
                {
                    result = typeConverter.ConvertFrom(null, cultureInfo, value);
                    return true;
                }
                catch
                {
                    // Ignore any exceptions.
                }
            }

            result = null;
            return false;
        }

        public static object ConvertToAnEnum(Type enumType, string value)
        {
            return Enum.Parse(enumType, RemoveWhitespace(value), true);
        }

        private static string RemoveWhitespace(string value)
        {
            return value.Replace(" ", string.Empty);
        }

        public static bool CanConvertSimple(IBindingType typeToConvertTo, object value, CultureInfo cultureInfo)
        {
            try
            {
                ConvertSimple(typeToConvertTo, value, cultureInfo);
                return true;
            }
            catch (InvalidCastException)
            {
                return false;
            }
            catch (OverflowException)
            {
                return false;
            }
            catch (FormatException)
            {
                return false;
            }
            catch (ArgumentException)
            {
                return false;
            }
        }
    }
}
