using System;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace TechTalk.SpecFlow
{
    internal class StepBinding
    {
        public BindingType Type { get; private set; }
        public Regex Regex { get; private set; }
        public Delegate BindingAction { get; private set; }
        public Type[] ParameterTypes { get; private set; }
        public MethodInfo MethodInfo { get; private set; }


        public StepBinding(BindingType type, Regex regex, Delegate bindingAction, Type[] parameterTypes, MethodInfo methodInfo)
        {
            Type = type;
            Regex = regex;
            BindingAction = bindingAction;
            ParameterTypes = parameterTypes;
            MethodInfo = methodInfo;
        }
    }
}