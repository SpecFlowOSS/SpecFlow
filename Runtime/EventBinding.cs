using System;
using System.Linq;
using System.Reflection;

namespace TechTalk.SpecFlow
{
    internal class EventBinding
    {
        public Delegate BindingAction { get; private set; }
        public string[] Tags { get; private set; }
        public MethodInfo MethodInfo { get; private set; }

        public EventBinding(Delegate bindingAction, string[] tags, MethodInfo methodInfo)
        {
            BindingAction = bindingAction;
            Tags = tags;
            MethodInfo = methodInfo;
        }
    }
}