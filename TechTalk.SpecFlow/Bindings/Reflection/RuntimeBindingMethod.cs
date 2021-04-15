using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace TechTalk.SpecFlow.Bindings.Reflection
{
    public class RuntimeBindingMethod : IBindingMethod
    {
        public readonly MethodInfo MethodInfo;

        public IBindingType Type
        {
            get { return new RuntimeBindingType(MethodInfo.DeclaringType); }
        }

        public IBindingType ReturnType
        {
            get { return MethodInfo.ReturnType == typeof(void) ? null : new RuntimeBindingType(MethodInfo.ReturnType); }
        }

        public string Name
        {
            get { return MethodInfo.Name; }
        }

        public BindingObsoletion Obsoletion { get; }

        public IEnumerable<IBindingParameter> Parameters
        {
            get { return MethodInfo.GetParameters().Select(pi => (IBindingParameter)new RuntimeBindingParameter(pi)); }
        }

        public RuntimeBindingMethod(MethodInfo methodInfo, ObsoleteAttribute obsoleteAttribute)
        {
            MethodInfo = methodInfo;
            Obsoletion = obsoleteAttribute is null ? BindingObsoletion.NotObsolete : new BindingObsoletion(this, obsoleteAttribute);
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
            if (obj.GetType() != this.GetType()) return false;
            return Equals((RuntimeBindingMethod) obj);
        }

        public override int GetHashCode()
        {
            return (MethodInfo != null ? MethodInfo.GetHashCode() : 0);
        }
    }
}