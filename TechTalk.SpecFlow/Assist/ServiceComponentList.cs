using System.Collections;
using System.Collections.Generic;

namespace TechTalk.SpecFlow.Assist
{
    public class ServiceComponentList<T> : IEnumerable<T>
    {
        private readonly IEnumerable<T> components;

        IEnumerator<T> IEnumerable<T>.GetEnumerator() => components.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)components).GetEnumerator();

        public ServiceComponentList()
        {
            components = new List<T>();
        }
    }
}