using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace TechTalk.SpecFlow.Assist
{
    public class ServiceComponentList<T> : IEnumerable<T>
    {
        private readonly List<T> components;

        private ValueHolder<T> defaultComponent;

        private IEnumerable<T> componentsWithDefault => components.Concat(defaultComponent);

        IEnumerator<T> IEnumerable<T>.GetEnumerator() => componentsWithDefault.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)componentsWithDefault).GetEnumerator();

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

        public void Unregister(T component)
        {
            if (!components.Remove(component))
            {
                if (defaultComponent.Contains(component))
                {
                    defaultComponent = ValueHolder<T>.Empty();
                }
            }
        }

        public void Unregister<TImpl>() where TImpl : T
        {
            componentsWithDefault.OfType<TImpl>().ToList().ForEach(component => Unregister(component));
        }

        public void Replace(T oldComponent, T newComponent)
        {
            Unregister(oldComponent);
            Register(newComponent);
        }

        public void Replace<TOld, TNew>() where TOld : T where TNew : T, new()
        {
            Unregister<TOld>();
            Register<TNew>();
        }

        public void SetDefault(T newComponent)
        {
            defaultComponent = ValueHolder<T>.WithValue(newComponent);
        }

        public void SetDefault<TImpl>() where TImpl : T, new()
        {
            SetDefault(new TImpl());
        }

        public void ClearDefault()
        {
            defaultComponent = ValueHolder<T>.Empty();
        }

        public void Clear()
        {
            components.Clear();
            ClearDefault();
        }
    }
}