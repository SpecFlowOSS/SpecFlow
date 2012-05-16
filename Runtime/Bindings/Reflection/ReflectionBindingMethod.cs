using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace TechTalk.SpecFlow.Bindings.Reflection
{
    public class ReflectionBindingMethod : IBindingMethod
    {
        public readonly MethodInfo MethodInfo;

        public IBindingType Type
        {
            get { return new ReflectionBindingType(MethodInfo.DeclaringType); }
        }

        public string Name
        {
            get { return MethodInfo.Name; }
        }

        public IEnumerable<IBindingParameter> Parameters
        {
            get { return MethodInfo.GetParameters().Select(pi => (IBindingParameter)new ReflectionBindingParameter(pi)); }
        }

        public ReflectionBindingMethod(MethodInfo methodInfo)
        {
            this.MethodInfo = methodInfo;
        }
    }
}