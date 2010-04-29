using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using TechTalk.SpecFlow.Tracing;

namespace TechTalk.SpecFlow.Bindings
{
    public interface IStepArgumentTypeConverter
    {
        object Convert(string value, Type typeToConvertTo, CultureInfo cultureInfo);
        bool CanConvert(string value, Type typeToConvertTo, CultureInfo cultureInfo);
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

        public object Convert(string value, Type typeToConvertTo, CultureInfo cultureInfo)
        {
            if (typeToConvertTo == typeof(string))
                return value;

            var stepTransformations = StepTransformations.Where(t => t.ReturnType == typeToConvertTo && t.Regex.IsMatch(value)).ToArray();
            Debug.Assert(stepTransformations.Length <= 1, "You may not call Convert if CanConvert returns false");
            if (stepTransformations.Length > 0)
                return stepTransformations[0].Transform(value);

            return ConvertSimple(typeToConvertTo, value, cultureInfo);
        }

        public bool CanConvert(string value, Type typeToConvertTo, CultureInfo cultureInfo)
        {
            if (typeToConvertTo == typeof(string))
                return true;

            var stepTransformations = StepTransformations.Where(t => t.ReturnType == typeToConvertTo && t.Regex.IsMatch(value)).ToArray();
            if (stepTransformations.Length > 1)
            {
                //TODO: error?
                testTracer.TraceWarning(string.Format("Multiple step transformation matches to the input ({0}, target type: {1}). We use the first.", value, typeToConvertTo));
            }
            if (stepTransformations.Length >= 1)
                return true;

            return CanConvertSimple(typeToConvertTo, value, cultureInfo);
        }

        private object ConvertSimple(Type typeToConvertTo, string value, CultureInfo cultureInfo)
        {
            if (typeToConvertTo.IsEnum)
                return Enum.Parse(typeToConvertTo, value, true);

            return System.Convert.ChangeType(value, typeToConvertTo, cultureInfo);
        }

        public bool CanConvertSimple(Type typeToConvertTo, string value, CultureInfo cultureInfo)
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