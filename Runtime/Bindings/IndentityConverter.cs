using System.Globalization;
using TechTalk.SpecFlow.Bindings.Reflection;

namespace TechTalk.SpecFlow.Bindings
{
    internal class IndentityConverter : IStepArgumentTypeConverter
    {
        public object Convert(object value, IBindingType typeToConvertTo, CultureInfo cultureInfo)
        {
            return value;
        }

        public bool CanConvert(object value, IBindingType typeToConvertTo, CultureInfo cultureInfo)
        {
            return typeToConvertTo.IsAssignableTo(value.GetType());
        }
    }
}
