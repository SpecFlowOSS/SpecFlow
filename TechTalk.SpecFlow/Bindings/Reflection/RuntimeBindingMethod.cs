using System;
using System.Collections.Generic;
using System.Reflection;

namespace TechTalk.SpecFlow.Bindings.Reflection
{
    public class RuntimeBindingMethod : IBindingMethod
    {
        public readonly MethodInfo MethodInfo;
        private RuntimeBindingType runtimeBindingType;
        private IBindingParameter[] _parameters;

        public IBindingType Type
        {
            get { return runtimeBindingType ??= new RuntimeBindingType(MethodInfo.DeclaringType); }
        }

        public IBindingType ReturnType
        {
            get { return MethodInfo.ReturnType == typeof(void) ? null : new RuntimeBindingType(MethodInfo.ReturnType); }
        }

        public string Name
        {
            get { return MethodInfo.Name; }
        }

        public IEnumerable<IBindingParameter> Parameters
        {
            get { return _parameters ??= GetParameters(); }
        }

        public RuntimeBindingMethod(MethodInfo methodInfo)
        {
            MethodInfo = methodInfo ?? throw new ArgumentNullException(nameof(methodInfo));
        }

        private IBindingParameter[] GetParameters()
        {
            var parameters = MethodInfo.GetParameters();
            if (parameters.Length == 0)
            {
                return Array.Empty<IBindingParameter>();
            }

            var result = new IBindingParameter[parameters.Length];
            for (int i = 0; i < parameters.Length; i++)
            {
                result[i] = new RuntimeBindingParameter(parameters[i]);
            }

            return result;
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
            return MethodInfo.GetHashCode();
        }
    }
}