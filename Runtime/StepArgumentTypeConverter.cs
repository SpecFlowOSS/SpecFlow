using System;
using System.Globalization;

namespace TechTalk.SpecFlow
{
    public interface IStepArgumentTypeConverter
    {
        object Convert(string value, Type typeToConvertTo, CultureInfo cultureInfo);
    }

    public class StepArgumentTypeConverter : IStepArgumentTypeConverter
    {
        public object Convert(string value, Type typeToConvertTo, CultureInfo cultureInfo)
        {
            object convertedArg;

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
    }
}