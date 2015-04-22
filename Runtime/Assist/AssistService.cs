using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoDi;
using TechTalk.SpecFlow.Assist.ValueComparers;
using TechTalk.SpecFlow.Bindings;
using TechTalk.SpecFlow.Bindings.Reflection;

namespace TechTalk.SpecFlow.Assist
{
    //WARNING: super stupid over simplified code!!!
    public class AssistService
    {
        private readonly IStepArgumentTypeConverter stepArgumentTypeConverter;
        private readonly IDictionary<string, IValueComparer> comparers;

        public AssistService(IStepArgumentTypeConverter stepArgumentTypeConverter, IDictionary<string, IValueComparer> comparers)
        {
            this.stepArgumentTypeConverter = stepArgumentTypeConverter;
            this.comparers = comparers;
        }

        public IEnumerable<T> CreateSet2<T>(Table table)
        {
            foreach (var tableRow in table.Rows)
            {
                var obj = (T)Activator.CreateInstance(typeof (T));
                foreach (var header in table.Header)
                {
                    var prop = typeof (T).GetProperty(header);
                    var convertedValue = stepArgumentTypeConverter.Convert(tableRow[header], new RuntimeBindingType(prop.PropertyType), CultureInfo.CurrentCulture);
                    prop.SetValue(obj, convertedValue);
                }
                yield return obj;
            }
        }

        public void CompareToSet<T>(Table table, IEnumerable<T> items)
        {
            //TODO: do comparison logic with "comparers"
        }
    }

    public static class AssistServiceExtensions
    {
        // looks like static, but is depends on ScenarioContext.Current, so it uses the current SpecFlow context
        // cannot be used for multi-threaded environment (not supported anyway yet), but for that, you can use Usage "A" pattern
        public static IEnumerable<T> CreateSet2<T>(this Table table)
        {
            var assistService = (AssistService) ScenarioContext.Current.GetBindingInstance(typeof (AssistService));
            return assistService.CreateSet2<T>(table);
        } 
    }

    public class DefaultAssistServiceContainerBuilder
    {
        public virtual void Build(IObjectContainer container)
        {
            container.RegisterTypeAs<BoolValueComparer, IValueComparer>("bool");
            container.RegisterTypeAs<DateTimeValueComparer, IValueComparer>("DateTime");
        }
    }
}
