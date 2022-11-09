namespace TechTalk.SpecFlow.Bindings;

public class BindingError
{
    public BindingErrorType Type { get; }
    public string Message { get; }

    public BindingError(BindingErrorType type, string message)
    {
        Type = type;
        Message = message;
    }
}

public enum BindingErrorType
{
    TypeLoadError,
    BindingError,
    StepDefinitionError,
}
