namespace TechTalk.SpecFlow.Bindings
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using TechTalk.SpecFlow.Assist;
    using TechTalk.SpecFlow.Bindings.Reflection;

    internal class HorizontalTableConverter : TableConverterBase
    {
        private readonly IStepArgumentTypeConverter stepArgumentTypeConverter;

        public HorizontalTableConverter(IStepArgumentTypeConverter stepArgumentTypeConverter)
        {
            this.stepArgumentTypeConverter = stepArgumentTypeConverter;
        }

        protected override object Convert(Table table, IBindingType typeToConvertTo, CultureInfo cultureInfo)
        {
            var itemRuntimeType = GetItemRuntimeType(typeToConvertTo);
            var rowFetcher = GetValidRowFetcherDelegate(table, itemRuntimeType, cultureInfo);
            var result = Array.CreateInstance(itemRuntimeType.Type, table.RowCount);

            for (int i = 0; i < result.Length; i++)
            {
                var objects = new Queue<object>();
                objects.Enqueue(rowFetcher(i));
                var item = stepArgumentTypeConverter.Convert(objects, itemRuntimeType, cultureInfo);
                result.SetValue(item, i);
            }

            return result;
        }

        protected override bool CanConvert(Table table, IBindingType typeToConvertTo, CultureInfo cultureInfo)
        {
            var itemRuntimeType = GetItemRuntimeType(typeToConvertTo);
            if (itemRuntimeType == null || IsVerticalTable(table, itemRuntimeType))
                return false;

            var rowFetcher = GetValidRowFetcherDelegate(table, itemRuntimeType, cultureInfo);
            return rowFetcher != null;
        }

        private RuntimeBindingType GetItemRuntimeType(IBindingType typeToConvertTo)
        {
            var runtimeType = typeToConvertTo as RuntimeBindingType;
            if (runtimeType == null)
                return null;

            var type = runtimeType.Type;

            if (type.IsArray && type.GetArrayRank() == 1)
                return new RuntimeBindingType(type.GetElementType());

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                return new RuntimeBindingType(type.GetGenericArguments()[0]);

            return null;
        }

        private Func<int, object> GetValidRowFetcherDelegate(Table table, IBindingType typeToConvertTo, CultureInfo cultureInfo)
        {
            if (table.RowCount == 0)
                return x => null;

            var pivotTable = new PivotTable(table);
            if (stepArgumentTypeConverter.CanConvert(CreateQueue(pivotTable.GetInstanceTable(0)), typeToConvertTo, cultureInfo))
                return x => pivotTable.GetInstanceTable(x);

            if (table.Header.Count == 1 &&
                stepArgumentTypeConverter.CanConvert(CreateQueue(table.Rows[0][0]), typeToConvertTo, cultureInfo))
                return x => table.Rows[x][0];

            return null;
        }

        private static Queue<object> CreateQueue(object value)
        {
            Queue<Object> queue = new Queue<object>();
            queue.Enqueue(value);
            return queue;
        }
    }
}