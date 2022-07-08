using System.Text.RegularExpressions;

namespace TechTalk.SpecFlow.Bindings
{
    /// <summary>
    /// Represents a custom step definition parameter binding
    /// </summary>
    public interface IStepArgumentTransformationBinding : IBinding
    {
        /// <summary>
        /// The optional name of the custom parameter. The name can be used in cucumber expressions.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The regular expression matches the step argument. Optional, if null, the transformation receives the entire argument.
        /// </summary>
        Regex Regex { get; }
    }
}