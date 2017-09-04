using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechTalk.SpecFlow.Assist
{
    public class TableDifferenceItem<TT>
    {
        public TT Item { get; private set; }
        public int? Index { get; private set; }

        public bool IsIndexSpecific => Index != null;

        public TableDifferenceItem(TT item, int? index = null)
        {
            Item = item;
            Index = index;
        }
    }
}
