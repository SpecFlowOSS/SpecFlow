using System.Globalization;
using System.Linq;
using TechTalk.SpecFlow.Bindings.Reflection;
using TechTalk.SpecFlow.Infrastructure;
using TechTalk.SpecFlow.Tracing;
using System.Collections.Generic;

namespace TechTalk.SpecFlow.Bindings
{
    public interface IStepArgumentTypeConverter
    {
        object Convert(object value, IBindingType typeToConvertTo, CultureInfo cultureInfo);
        bool CanConvert(object value, IBindingType typeToConvertTo, CultureInfo cultureInfo);
    }

    public class StepArgumentTypeConverter : IStepArgumentTypeConverter
    {
        private readonly IEnumerable<IStepArgumentTypeConverter> converters;

        public StepArgumentTypeConverter(ITestTracer testTracer, IBindingRegistry bindingRegistry, IContextManager contextManager, IBindingInvoker bindingInvoker)
        {
            converters = new IStepArgumentTypeConverter[]
            {
                new IndentityConverter(),
                new StepArgumentTransformationConverter(this, testTracer, bindingRegistry, contextManager, bindingInvoker),
                new VerticalTableConverter(this),
                new HorizontalTableConverter(this),
                new SimpleConverter()
            };
        }

        public object Convert(object value, IBindingType typeToConvertTo, CultureInfo cultureInfo)
        {
            var converter = GetConverter(value, typeToConvertTo, cultureInfo);
            return converter.Convert(value, typeToConvertTo, cultureInfo);
        }

        public bool CanConvert(object value, IBindingType typeToConvertTo, CultureInfo cultureInfo)
        {
            return GetConverter(value, typeToConvertTo, cultureInfo) != null;
        }

        private IStepArgumentTypeConverter GetConverter(object value, IBindingType typeToConvertTo, CultureInfo cultureInfo)
        {
            return converters.FirstOrDefault(x => x.CanConvert(value, typeToConvertTo, cultureInfo));
        }
    }
}
