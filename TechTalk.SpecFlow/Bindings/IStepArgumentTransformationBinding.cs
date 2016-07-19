using System.Text.RegularExpressions;

namespace TechTalk.SpecFlow.Bindings
{
    public interface IStepArgumentTransformationBinding : IBinding
    {
        Regex Regex { get; }
    }
}