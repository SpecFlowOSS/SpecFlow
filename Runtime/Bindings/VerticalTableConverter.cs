using System;
using System.Collections.Generic;
using System.Linq;
using TechTalk.SpecFlow.Bindings.Reflection;
using System.Globalization;
using System.Reflection;
using TechTalk.SpecFlow.Assist;

namespace TechTalk.SpecFlow.Bindings
{
    internal class VerticalTableConverter : TableConverterBase
    {
        private readonly IStepArgumentTypeConverter stepArgumentTypeConverter;

        public VerticalTableConverter(IStepArgumentTypeConverter stepArgumentTypeConverter)
        {
            this.stepArgumentTypeConverter = stepArgumentTypeConverter;
        }

        protected override object Convert(Table table, IBindingType typeToConvertTo, CultureInfo cultureInfo)
        {
            var runtimeBindingType = (RuntimeBindingType)typeToConvertTo;
            var verticalTable = GetVerticalTable(table, runtimeBindingType);
            var values = GetValues(verticalTable);
            var objectInitializationData = GetObjectInitializationData(values.Keys, runtimeBindingType.Type);
            var bindingTypes = objectInitializationData.GetBindingTypes();

            var convertedValues = values.ToDictionary(
                x => x.Key,
                x => stepArgumentTypeConverter.Convert(x.Value, bindingTypes[x.Key], cultureInfo));
            var constructorParameters = objectInitializationData.Constructor.GetParameters()
                .Select(x => convertedValues[SanitizePropertyName(x.Name)]).ToArray();
            var result = objectInitializationData.Constructor.Invoke(constructorParameters);

            foreach (var property in objectInitializationData.PropertiesToSet)
            {
                var value = convertedValues[SanitizePropertyName(property.Name)];
                property.SetValue(result, value, null);
            }

            return result;
        }

        protected override bool CanConvert(Table table, IBindingType typeToConvertTo, CultureInfo cultureInfo)
        {
            var runtimeBindingType = typeToConvertTo as RuntimeBindingType;
            if (runtimeBindingType == null)
                return false;

            var verticalTable = GetVerticalTable(table, runtimeBindingType);
            if (verticalTable == null)
                return false;

            var values = GetValues(verticalTable);
            var objectInitializationData = GetObjectInitializationData(values.Keys, runtimeBindingType.Type);
            if (objectInitializationData == null)
                return false;

            var bindingTypes = objectInitializationData.GetBindingTypes();
            return values.All(x => stepArgumentTypeConverter.CanConvert(x.Value, bindingTypes[x.Key], cultureInfo));
        }

        private Table GetVerticalTable(Table table, RuntimeBindingType runtimeBindingType)
        {
            if (IsVerticalTable(table, runtimeBindingType))
                return table;

            if (table.Rows.Count == 1)
            {
                var pivotedTable = new PivotTable(table).GetInstanceTable(0);

                if (IsVerticalTable(pivotedTable, runtimeBindingType))
                    return pivotedTable;
            }

            return null;
        }

        private Dictionary<string, string> GetValues(Table table)
        {
            return table.Rows.ToDictionary(x => SanitizePropertyName(x[0]), x => x[1]);
        }

        private ObjectInitializationData GetObjectInitializationData(IEnumerable<string> columns, Type type)
        {
            var writableProperties = GetWritableProperties(type);
            var readonlyColumns = new HashSet<string>(columns.Where(x => !writableProperties.ContainsKey(x)));
            var writableColumns = new HashSet<string>(columns.Where(writableProperties.ContainsKey));

            var constructor = GetConstructor(type, readonlyColumns, writableColumns);
            if (constructor == null)
                return null;

            var constructorColumns = new HashSet<string>(constructor.GetParameters().Select(x => SanitizePropertyName(x.Name)));
            var propertiesToSet = columns.Where(x => !constructorColumns.Contains(x)).Select(x => writableProperties[x]).ToList();

            return new ObjectInitializationData
            {
                Constructor = constructor,
                PropertiesToSet = propertiesToSet
            };
        }

        private ConstructorInfo GetConstructor(Type type, HashSet<string> requiredParameters, HashSet<string> optionalParameters)
        {
            var constructors = type.GetConstructors().OrderBy(x => x.GetParameters().Length);
            foreach (var constructor in constructors)
            {
                var parameterNames = constructor.GetParameters().Select(x => SanitizePropertyName(x.Name)).ToList();
                var requiredCount = parameterNames.Count(requiredParameters.Contains);
                if (requiredCount != requiredParameters.Count)
                    continue;

                var optionalCount = parameterNames.Count(optionalParameters.Contains);
                if (parameterNames.Count == requiredCount + optionalCount)
                    return constructor;
            }

            return null;
        }

        private class ObjectInitializationData
        {
            public ConstructorInfo Constructor { get; set; }

            public List<PropertyInfo> PropertiesToSet { get; set; }

            public Dictionary<string, IBindingType> GetBindingTypes()
            {
                var bindingTypes = new Dictionary<string, IBindingType>();
                foreach (var parameter in Constructor.GetParameters())
                {
                    bindingTypes.Add(SanitizePropertyName(parameter.Name), new RuntimeBindingType(parameter.ParameterType));
                }
                foreach (var property in PropertiesToSet)
                {
                    bindingTypes.Add(SanitizePropertyName(property.Name), new RuntimeBindingType(property.PropertyType));
                }
                return bindingTypes;
            }
        }
    }
}
