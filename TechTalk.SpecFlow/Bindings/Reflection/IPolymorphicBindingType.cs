namespace TechTalk.SpecFlow.Bindings.Reflection
{
    public interface IPolymorphicBindingType : IBindingType
    {
        bool IsAssignableTo(IBindingType baseType);
    }
}