namespace TechTalk.SpecFlow.Bindings
{
    using System.Collections.Generic;
    using System.Globalization;
    using TechTalk.SpecFlow.Bindings.Reflection;

    public interface IStepArgumentTypeConverter
    {
        object Convert(Queue<object> values, IBindingType typeToConvertTo, CultureInfo cultureInfo);
        bool CanConvert(Queue<object> values, IBindingType typeToConvertTo, CultureInfo cultureInfo);
    }
}