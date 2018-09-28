using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace TechTalk.SpecFlow.Assist
{
    public class ServiceComponentList<T> : IEnumerable<T>
    {
        private readonly List<T> components;

        private ValueHolder<T> @default;

        private IEnumerable<T> componentsWithDefault => components.Concat(@default);

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
                if (@default.Contains(component))
                {
                    @default = ValueHolder.Empty<T>();
                }
            }
        }

        public void Unregister<TImpl>() where TImpl : T
        {
            componentsWithDefault.OfType<TImpl>().ToList().ForEach(component => Unregister(component));
        }

        public void Replace(T old, T @new)
        {
            Unregister(old);
            Register(@new);
        }

        public void Replace<TOld, TNew>() where TOld : T where TNew : T, new()
        {
            Unregister<TOld>();
            Register<TNew>();
        }

        public void SetDefault(T @new)
        {
            @default = ValueHolder.WithValue(@new);
        }

        public void SetDefault<TImpl>() where TImpl : T, new()
        {
            SetDefault(new TImpl());
        }
    }
}