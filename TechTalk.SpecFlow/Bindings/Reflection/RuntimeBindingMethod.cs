using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace TechTalk.SpecFlow.Bindings.Reflection
{
    public class RuntimeBindingMethod : IBindingMethod
    {
        public readonly MethodInfo MethodInfo;

        public IBindingType Type => new RuntimeBindingType(MethodInfo.DeclaringType);

        public IBindingType ReturnType => MethodInfo.ReturnType == typeof(void) ? null : new RuntimeBindingType(MethodInfo.ReturnType);

        public string Name => MethodInfo.Name;

        public IEnumerable<IBindingParameter> Parameters
        {
            get { return MethodInfo.GetParameters().Select(pi => (IBindingParameter)new RuntimeBindingParameter(pi)); }
        }

        public RuntimeBindingMethod(MethodInfo methodInfo)
        {
            MethodInfo = methodInfo ?? throw new ArgumentNullException(nameof(methodInfo));
        }

        public override string ToString()
        {
            return this.GetShortDisplayText();
        }

        protected bool Equals(RuntimeBindingMethod other)
        {
            return Equals(MethodInfo, other.MethodInfo);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((RuntimeBindingMethod) obj);
        }

        public override int GetHashCode()
        {
            return (MethodInfo != null ? MethodInfo.GetHashCode() : 0);
        }
    }
}