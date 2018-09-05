using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace TechTalk.SpecFlow.Assist
{
    public class ServiceComponentList<T> : IEnumerable<T>
    {
        private readonly List<T> components;

        IEnumerator<T> IEnumerable<T>.GetEnumerator() => components.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)components).GetEnumerator();

        public ServiceComponentList()
        {
            components = new List<T>();
        }

        public void Register(T component)
        {
            components.Insert(0, component);
        }

        public void Register<TImpl>() where TImpl : T, new()
        {
            Register(new TImpl());
        }

        public void RegisterDefault(T component)
        {
            components.Add(component);
        }

        public void Unregister(T component)
        {
            components.Remove(component);
        }

        public void Unregister<TImpl>() where TImpl : T
        {
            components.OfType<TImpl>().ToList().ForEach(component => components.Remove(component));
        }
    }
}