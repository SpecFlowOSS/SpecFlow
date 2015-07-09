namespace TechTalk.SpecFlow.Bindings
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using TechTalk.SpecFlow.Assist.ValueRetrievers;
    using TechTalk.SpecFlow.Bindings.Reflection;

    internal class SimpleConverter : IStepArgumentTypeConverter
    {
        public object Convert(Queue<object> values, IBindingType typeToConvertTo, CultureInfo cultureInfo)
        {
            var value = values.Dequeue();
            var runtimeType = (RuntimeBindingType)typeToConvertTo;

            if (runtimeType.Type.IsEnum && value is string)
                return Enum.Parse(runtimeType.Type, ((string)value).Replace(" ", ""), true);

            if (runtimeType.Type == typeof(Guid?) && string.IsNullOrEmpty(value as string))
                return null;

            if (runtimeType.Type == typeof(Guid) || runtimeType.Type == typeof(Guid?))
                return new GuidValueRetriever().GetValue(value as string);

            return System.Convert.ChangeType(value, runtimeType.Type, cultureInfo);
        }

        public bool CanConvert(Queue<object> values, IBindingType typeToConvertTo, CultureInfo cultureInfo)
        {
            var value = values.Peek();
            var runtimeType = typeToConvertTo as RuntimeBindingType;

            if (runtimeType == null)
            {
                return false;
            }

            var targetType = runtimeType.Type;

            if (value == null)
            {
                return !targetType.IsValueType;
            }

            if (!targetType.IsEnum || targetType != typeof(Guid) || targetType != typeof(Guid?))
            {
                var convertible = value as IConvertible;
                if (convertible == null)
                {
                    return value.GetType() == targetType;
                }
            }

            try
            {
                Convert(CreateQueue(value), typeToConvertTo, cultureInfo);
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

        private static Queue<object> CreateQueue(object value)
        {
            Queue<Object> queue = new Queue<object>();
            queue.Enqueue(value);
            return queue;
        }
    }
}