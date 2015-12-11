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
        private static ITableServices currentTmp; //TODO[assistcont]: to be eliminated, should be stored in the container
        public static ITableServices Current
        {
            get
            {
                if (currentTmp == null)
                    currentTmp = new TableServices();
                //TODO[assistcont]: Get it from current scenario context
                return currentTmp;
            }
        }

        internal Service Service { get; private set; } //TODO[assistcont]: Merge Service into TableServices

        private TableServices() : this(ScenarioContext.Current == null ? null : ScenarioContext.Current.ScenarioContainer)
        {
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
