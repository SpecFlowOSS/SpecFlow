using TechTalk.SpecFlow.Bindings.Reflection;

namespace TechTalk.SpecFlow.Bindings.Discovery
{
    public class BindingSourceType
    {
        public IBindingType BindingType { get; set; }

        public bool IsClass { get; set; }
        public bool IsPublic { get; set; }
        public bool IsAbstract { get; set; }
        public bool IsGenericTypeDefinition { get; set; }
        public bool IsNested { get; set; }

        public BindingSourceAttribute[] Attributes { get; set; }
    }
}