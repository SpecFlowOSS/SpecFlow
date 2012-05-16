using System.Reflection;

namespace TechTalk.SpecFlow.Bindings.Reflection
{
    public class ReflectionBindingParameter : IBindingParameter
    {
        private readonly ParameterInfo parameterInfo;

        public IBindingType Type
        {
            get { return new ReflectionBindingType(parameterInfo.ParameterType); }
        }

        public string ParameterName
        {
            get { return parameterInfo.Name; }
        }

        public ReflectionBindingParameter(ParameterInfo parameterInfo)
        {
            this.parameterInfo = parameterInfo;
        }
    }
}