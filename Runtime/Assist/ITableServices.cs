using System.Collections.Generic;

namespace TechTalk.SpecFlow
{
    public interface ITableServices
    {
        IEnumerable<T> CreateSet<T>(Table table);
    }
}