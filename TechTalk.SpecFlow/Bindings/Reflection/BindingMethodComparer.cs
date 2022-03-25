using System.Collections.Generic;

namespace TechTalk.SpecFlow.Bindings.Reflection
{
    public class BindingMethodComparer : IEqualityComparer<IBindingMethod>
    {
        public static readonly BindingMethodComparer Instance = new();

        public bool Equals(IBindingMethod x, IBindingMethod y)
        {
            return x.MethodEquals(y);
        }

        public int GetHashCode(IBindingMethod obj)
        {
            return obj.Name.GetHashCode();
        }
    }
}