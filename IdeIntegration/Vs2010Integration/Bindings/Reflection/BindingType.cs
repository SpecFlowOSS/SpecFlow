namespace TechTalk.SpecFlow.Bindings.Reflection
{
    public class BindingReflectionType : IBindingType
    {
        public string Name { get; private set; }
        public string FullName { get; private set; }

        public BindingReflectionType(string name, string fullName)
        {
            Name = name;
            FullName = fullName;
        }
    }
}