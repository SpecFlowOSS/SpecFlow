using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoDi;

namespace TechTalk.SpecFlow.Assist
{
    public class TableServices : ITableServices
    {
        private static readonly Lazy<ITableServices> contextIndependentInstance = new Lazy<ITableServices>(() => new TableServices(null), true);
        public static ITableServices Current
        {
            get
            {
                var container = GetCurrentContainer();
                if (container == null)
                    return contextIndependentInstance.Value;
                return container.Resolve<TableServices>();
            }
        }

        internal Service Service { get; private set; } //TODO[assistcont]: Merge Service into TableServices

        private static IObjectContainer GetCurrentContainer()
        {
            var scenarioContext = ScenarioContext.Current;
            return scenarioContext == null ? null : scenarioContext.ScenarioContainer;
        }

        public TableServices(IObjectContainer container)
        {
            Service = new Service(container);
        }

        public IEnumerable<T> CreateSet<T>(Table table)
        {
            var list = new List<T>();

            var pivotTable = new PivotTable(table);
            for (var index = 0; index < table.Rows.Count(); index++)
            {
                var instance = pivotTable.GetInstanceTable(index).CreateInstance<T>();
                list.Add(instance);
            }

            return list;
        }
    }
}
