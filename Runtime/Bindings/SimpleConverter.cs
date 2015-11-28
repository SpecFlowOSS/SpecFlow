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

            var targetNullableType = Nullable.GetUnderlyingType(runtimeType.Type);
            // if property is nullable type
            if (targetNullableType != null)
            {
                var stringVal = value as string;

                // When .NET 4.0 or higher use string.IsNullOrWhiteSpace instead
                if (string.IsNullOrEmpty(stringVal) || stringVal.Trim() == string.Empty)
                    return null;

                return System.Convert.ChangeType(stringVal, targetNullableType, cultureInfo);
            }

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
