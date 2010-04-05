using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace TechTalk.SpecFlow
{
    public interface IStepArgumentTypeConverter
    {
        ICollection<StepTransformation> StepTransformations { get; }
        object Convert(string value, Type typeToConvertTo, CultureInfo cultureInfo);
        bool CanConvert(string value, Type typeToConvertTo, CultureInfo cultureInfo);
    }

    public class StepArgumentTypeConverter : IStepArgumentTypeConverter
    {
        public StepArgumentTypeConverter(ICollection<StepTransformation> stepTransformations)
        {
            StepTransformations = stepTransformations ?? new List<StepTransformation>();
        }

        public ICollection<StepTransformation> StepTransformations { get; private set; }

        public object Convert(string value, Type typeToConvertTo, CultureInfo cultureInfo)
        {
            object convertedArg;

            var stepTransformation = StepTransformations.Where(t => t.ReturnType == typeToConvertTo && t.Regex.IsMatch(value)).SingleOrDefault();
            if (stepTransformation != null)
            {
                var arguments = stepTransformation.GetStepTransformationArguments(value);
                var convertedValue = stepTransformation.BindingAction.DynamicInvoke(arguments);
                return convertedValue;
            }

            var paramType = typeToConvertTo;
            if (paramType.IsEnum)
            {
                convertedArg = Enum.Parse(paramType, value, true);
            }
            else
            {
                convertedArg = System.Convert.ChangeType(value, paramType, cultureInfo);
            }

            return convertedArg;
        }

        public bool CanConvert(string value, Type typeToConvertTo, CultureInfo cultureInfo)
        {
            try
            {
                var val = Convert(value, typeToConvertTo, cultureInfo);
                if (val != null)
                    return true;
            }
            catch (Exception)
            {
                return false;
            }
            return false;
        }
    }
}