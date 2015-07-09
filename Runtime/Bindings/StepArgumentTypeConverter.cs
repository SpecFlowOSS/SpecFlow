namespace TechTalk.SpecFlow.Bindings
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using TechTalk.SpecFlow.Bindings.Reflection;
    using TechTalk.SpecFlow.Infrastructure;
    using TechTalk.SpecFlow.Tracing;

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

        public object Convert(Queue<object> values, IBindingType typeToConvertTo, CultureInfo cultureInfo)
        {
            var converter = GetConverter(values, typeToConvertTo, cultureInfo);
            return converter.Convert(values, typeToConvertTo, cultureInfo);
        }

        public bool CanConvert(Queue<object> values, IBindingType typeToConvertTo, CultureInfo cultureInfo)
        {
            return GetConverter(values, typeToConvertTo, cultureInfo) != null;
        }

        private IStepArgumentTypeConverter GetConverter(Queue<object> values, IBindingType typeToConvertTo, CultureInfo cultureInfo)
        {
            return converters.FirstOrDefault(x => x.CanConvert(values, typeToConvertTo, cultureInfo));
        }
    }
}