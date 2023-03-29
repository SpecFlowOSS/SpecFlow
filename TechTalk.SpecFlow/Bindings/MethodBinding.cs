using System;
using TechTalk.SpecFlow.Bindings.Reflection;

namespace TechTalk.SpecFlow.Bindings
{
    public interface IBinding
    {
        IBindingMethod Method { get; }
    }

    public abstract class MethodBinding : IBinding
    {
        public IBindingMethod Method { get; private set; }

        protected MethodBinding(IBindingMethod bindingMethod)
        {
            Method = bindingMethod;
        }

        protected bool Equals(MethodBinding other)
        {
            if (Method == null )
                if (other.Method == null)
                    return true;
                else
                    return false;

            if (Method != null)
                if (other.Method == null)
                    return false;
                else
                    return Method.Equals(other.Method);

            return false;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((MethodBinding) obj);
        }

        public override int GetHashCode()
        {
            return (Method != null ? Method.GetHashCode() : 0);
        }

        internal Delegate cachedBindingDelegate;
    }
}