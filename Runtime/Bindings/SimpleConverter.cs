using System;
using System.Globalization;
using TechTalk.SpecFlow.Assist.ValueRetrievers;
using TechTalk.SpecFlow.Bindings.Reflection;

namespace TechTalk.SpecFlow.Bindings
{
    internal class SimpleConverter : IStepArgumentTypeConverter
    {
        public object Convert(object value, IBindingType typeToConvertTo, CultureInfo cultureInfo)
        {
            var runtimeType = (RuntimeBindingType)typeToConvertTo;

            if (runtimeType.Type.IsEnum && value is string)
                return Enum.Parse(runtimeType.Type, ((string)value).Replace(" ", ""), true);

            if (runtimeType.Type == typeof(Guid?) && string.IsNullOrEmpty(value as string))
                return null;

            if (runtimeType.Type == typeof(Guid) || runtimeType.Type == typeof(Guid?))
                return new GuidValueRetriever().GetValue(value as string);

            return System.Convert.ChangeType(value, runtimeType.Type, cultureInfo);
        }

        public bool CanConvert(object value, IBindingType typeToConvertTo, CultureInfo cultureInfo)
        {
            if (!(typeToConvertTo is RuntimeBindingType))
                return false;

            try
            {
                Convert(value, typeToConvertTo, cultureInfo);
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
