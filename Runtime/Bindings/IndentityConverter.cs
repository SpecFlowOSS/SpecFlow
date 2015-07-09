namespace TechTalk.SpecFlow.Bindings
{
    using System.Collections.Generic;
    using System.Globalization;
    using TechTalk.SpecFlow.Bindings.Reflection;

    internal class IndentityConverter : IStepArgumentTypeConverter
    {
        public object Convert(Queue<object> values, IBindingType typeToConvertTo, CultureInfo cultureInfo)
        {
            return values.Dequeue();
        }

        public bool CanConvert(Queue<object> values, IBindingType typeToConvertTo, CultureInfo cultureInfo)
        {
            var isAssignableTo = typeToConvertTo.IsAssignableTo(values.Peek().GetType());
            return isAssignableTo;
        }
    }
}