using System.Collections;
using System.Collections.Generic;

namespace TechTalk.SpecFlow.Assist
{
    #nullable enable
    public class ServiceComponentList<T> : IEnumerable<T>
    {
        private readonly List<T> components;
        private T? defaultComponent;

        public List<T>.Enumerator GetEnumerator() => components.GetEnumerator();
        IEnumerator<T> IEnumerable<T>.GetEnumerator() => components.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)components).GetEnumerator();

        public ServiceComponentList()
        {
            components = new List<T>();
        }

        internal ServiceComponentList(List<T> initialList, bool isLastOneTheDefault)
        {
            components = initialList;
            if (isLastOneTheDefault && initialList.Count > 0)
            {
                defaultComponent = initialList[initialList.Count - 1];
            }
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
            if (components.Remove(component))
            {
                if (ReferenceEquals(defaultComponent, component))
                {
                    defaultComponent = default;
                }
            }
        }

        public void Unregister<TImpl>() where TImpl : T
        {
            for (var index = components.Count - 1; index >= 0; index--)
            {
                var component = components[index];
                if (component is TImpl)
                {
                    Unregister(component);
                }
            }
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
            ClearDefault();
            components.Add(newComponent);
            defaultComponent = newComponent;
        }

        public void SetDefault<TImpl>() where TImpl : T, new()
        {
            SetDefault(new TImpl());
        }

        public void ClearDefault()
        {
            if (defaultComponent is not null)
            {
                Unregister(defaultComponent);
            }
        }

        public void Clear()
        {
            components.Clear();
            defaultComponent = default;
        }
    }
}