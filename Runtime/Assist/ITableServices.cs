using System.Collections.Generic;

namespace TechTalk.SpecFlow.Assist
{
    public interface ITableServices
    {
        IEnumerable<T> CreateSet<T>(Table table);
    }
}