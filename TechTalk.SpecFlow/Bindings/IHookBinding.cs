namespace TechTalk.SpecFlow.Bindings
{
    public interface IHookBinding : IScopedBinding, IBinding
    {
        HookType HookType { get; }
        int HookOrder { get; }
    }
}