using System;
using System.Collections.Generic;
using System.Linq;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class EnumArrayValueRetriever : IValueRetriever
    {
        public virtual object GetValue(string value, Type targetEnumType)
        {
            var enums = new StringArrayValueRetriever()
                .GetValue(value)
                .Select(item => Enum.Parse(targetEnumType, item))
                .ToArray(); 

            Array dest = Array.CreateInstance(targetEnumType, enums.Length);
            Array.Copy(enums, dest, enums.Length);

            return dest;

        }

        public object Retrieve(KeyValuePair<string, string> keyValuePair, Type targetType, Type propertyType)
        {
            return GetValue(keyValuePair.Value, propertyType.GetElementType());
        }

        public bool CanRetrieve(KeyValuePair<string, string> keyValuePair, Type targetType, Type propertyType)
        {
            return propertyType.IsArray && propertyType.GetElementType().IsEnum;
        }

        Array CastArray(Type target, string[] input)
        {
            Array dest = Array.CreateInstance(target, input.Length);

            var enums = input.Select(value => Enum.Parse(target, value)).ToArray();

            Array.Copy(enums, dest, enums.Length);

            return dest;
        }
    }
}