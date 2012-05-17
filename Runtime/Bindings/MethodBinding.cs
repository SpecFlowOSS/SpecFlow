using System;
using System.Linq;
using System.Reflection;
using TechTalk.SpecFlow.Bindings.Reflection;

namespace TechTalk.SpecFlow.Bindings
{
    public interface IBinding
    {
        IBindingMethod Method { get; }
    }

    public abstract class MethodBinding : IBinding
    {
        protected MethodBinding(IBindingMethod bindingMethod)
        {
            Method = bindingMethod;
        }

        public IBindingMethod Method { get; private set; }

        public Type[] ParameterTypes { get { return AssertMethodInfo().GetParameters().Select(pi => pi.ParameterType).ToArray(); } }
        public Type ReflectionReturnType { get { return AssertMethodInfo().ReturnType; } }

        internal Delegate cachedBindingDelegate;
        internal protected MethodInfo AssertMethodInfo()
        {
            var reflectionBindingMethod = Method as ReflectionBindingMethod;
            if (reflectionBindingMethod == null)
                throw new SpecFlowException("The binding method cannot be used for reflection: " + Method);
            return reflectionBindingMethod.MethodInfo;
        }
    }
}