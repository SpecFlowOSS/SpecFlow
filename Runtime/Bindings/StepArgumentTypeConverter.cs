using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TechTalk.SpecFlow.Tracing;

namespace TechTalk.SpecFlow.Bindings
{
    public interface IStepArgumentTypeConverter
    {
        object Convert(object value, Type typeToConvertTo, CultureInfo cultureInfo);
        bool CanConvert(object value, Type typeToConvertTo, CultureInfo cultureInfo);
    }

    public class StepArgumentTypeConverter : IStepArgumentTypeConverter
    {
        private readonly ITestTracer testTracer;

        public StepArgumentTypeConverter()
        {
            testTracer = ObjectContainer.TestTracer;
            StepTransformations = ObjectContainer.BindingRegistry.StepTransformations ?? new List<StepTransformationBinding>();
        }

        public ICollection<StepTransformationBinding> StepTransformations { get; private set; }

        private StepTransformationBinding GetMatchingStepTransformation(object value, Type typeToConvertTo, bool traceWarning)
        {
            var stepTransformations = StepTransformations.Where(t => CanConvert(t, value, typeToConvertTo)).ToArray();
            if (stepTransformations.Length > 1 && traceWarning)
            {
                testTracer.TraceWarning(string.Format("Multiple step transformation matches to the input ({0}, target type: {1}). We use the first.", value, typeToConvertTo));
            }

            return stepTransformations.Length > 0 ? stepTransformations[0] : null;
        }

        public object Convert(object value, Type typeToConvertTo, CultureInfo cultureInfo)
        {
            if (value == null) throw new ArgumentNullException("value");

            if (typeToConvertTo == value.GetType())
                return value;

            var stepTransformation = GetMatchingStepTransformation(value, typeToConvertTo, true);
            if (stepTransformation != null)
                return stepTransformation.Transform(value, testTracer);

            return ConvertSimple(typeToConvertTo, value, cultureInfo);
        }

        public bool CanConvert(object value, Type typeToConvertTo, CultureInfo cultureInfo)
        {
            if (value == null) throw new ArgumentNullException("value");

            if (typeToConvertTo == value.GetType())
                return true;

            var stepTransformation = GetMatchingStepTransformation(value, typeToConvertTo, false);
            if (stepTransformation != null)
                return true;

            return CanConvertSimple(typeToConvertTo, value, cultureInfo);
        }

        private bool CanConvert(StepTransformationBinding stepTransformationBinding, object value, Type typeToConvertTo)
        {
            if (stepTransformationBinding.ReturnType != typeToConvertTo)
                return false;

            if (stepTransformationBinding.Regex != null && value is string)
                return stepTransformationBinding.Regex.IsMatch((string) value);
            return true;
        }

        private object ConvertSimple(Type typeToConvertTo, object value, CultureInfo cultureInfo)
        {
            if (typeToConvertTo.IsEnum && value is string)
                return Enum.Parse(typeToConvertTo, (string)value, true);

            return System.Convert.ChangeType(value, typeToConvertTo, cultureInfo);
        }

        public bool CanConvertSimple(Type typeToConvertTo, object value, CultureInfo cultureInfo)
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
