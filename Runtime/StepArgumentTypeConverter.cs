using System;

namespace TechTalk.SpecFlow
{
    public interface IStepArgumentTypeConverter
    {
        object Convert(string value, Type typeToConvertTo);
    }

    public class StepStepArgumentTypeConverter : IStepArgumentTypeConverter
    {
        public object Convert(string value, Type typeToConvertTo)
        {
            object convertedArg;

            var paramType = typeToConvertTo;
            if (paramType.BaseType == typeof(Enum))
            {
                convertedArg = Enum.Parse(paramType, value, true);
            }
            else
            {
                convertedArg = System.Convert.ChangeType(value, paramType);
            }

            return convertedArg;
        }
    }
}