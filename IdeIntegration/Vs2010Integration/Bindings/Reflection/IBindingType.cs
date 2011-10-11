using System;

namespace TechTalk.SpecFlow.Bindings.Reflection
{
    public interface IBindingType
    {
        string Name { get; }
        string FullName { get; }
    }
}