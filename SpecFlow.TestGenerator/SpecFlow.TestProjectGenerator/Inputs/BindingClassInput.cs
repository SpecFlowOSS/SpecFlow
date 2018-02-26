using System.Collections.Generic;

namespace SpecFlow.TestProjectGenerator.Inputs
{
    public class BindingClassInput : FileInput
    {
        public BindingClassInput(string fileName, string folder = ".")
            : base(fileName, folder)
        {
            StepBindings = new List<StepBindingInput>();
            OtherBindings = new List<string>();
        }

        public List<StepBindingInput> StepBindings { get; }
        public List<string> OtherBindings { get; }
    }
}